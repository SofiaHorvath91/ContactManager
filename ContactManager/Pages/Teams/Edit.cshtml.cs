using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ContactManager.Pages.Teams
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        [BindProperty]
        public Team Team { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        public List<ExistingUniversesData> ExistingUniversesList;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Team = await Context.Teams
                      .Include(s => s.TeamEntries)
                          .ThenInclude(e => e.Character)
                      .Include(s => s.OpponentEntries)
                          .ThenInclude(e => e.Character)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Universe)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Character)
                      .AsNoTracking()
                      .FirstOrDefaultAsync(m => m.TeamID == id);

            if (Team == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                      User, Team,
                                                      ModelsOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            PopulateExistingUniverses(Team);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedUniverses)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var team = await Context.Teams
                      .Include(s => s.TeamEntries)
                          .ThenInclude(e => e.Character)
                      .Include(s => s.OpponentEntries)
                          .ThenInclude(e => e.Character)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Universe)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Character)
                      .FirstOrDefaultAsync(m => m.TeamID == id);

            if (team == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, team,
                                                     ModelsOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Team.OwnerID = team.OwnerID;
            Context.Entry(team).State = EntityState.Detached;
            Context.Attach(Team).State = EntityState.Modified;

            if (Team.Status == TeamStatus.Approved)
            {
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        Team,
                                        ModelsOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    Team.Status = TeamStatus.Submitted;
                }
            }

            Context.Entry(Team).State = EntityState.Detached;
            Context.Entry(team).State = EntityState.Modified;

            using (var memoryStream = new MemoryStream())
            {
                if (FileUpload?.Length > 0)
                {
                    await FileUpload.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 524288)
                    {
                        team.ProfileImage = memoryStream.ToArray();
                        await Context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                        return Page();
                    }
                }
                else
                {
                    var path = Path.Combine(Environment.WebRootPath, "img", $"profilepic.png");
                    team.ProfileImage = team.ProfileImage.Length != 0 ? team.ProfileImage : System.IO.File.ReadAllBytes(path);
                    await Context.SaveChangesAsync();
                }
            }

            if (await TryUpdateModelAsync<Team>(
               team,
               "Team",
               s => s.TeamName, s => s.TeamMotto, s => s.Introduction, s => s.Side))
            {
                UpdateTeamUniverses(selectedUniverses, team);
                await Context.SaveChangesAsync();
                return RedirectToPage("./Details", new { id = team.TeamID });
            }

            UpdateTeamUniverses(selectedUniverses, team);
            PopulateExistingUniverses(Team);
            return Page();
        }

        public void PopulateExistingUniverses(Team team)
        {
            var allUniverses = Context.Universes
                              .Include(s => s.UniverseEntries)
                                  .ThenInclude(e => e.Character)
                              .Include(s => s.UniverseEntries)
                                  .ThenInclude(e => e.Team);
            var teamUniverses = new HashSet<int>(team.UniverseEntries.Select(c => c.UniverseID));
            ExistingUniversesList = new List<ExistingUniversesData>();
            foreach (var universe in allUniverses)
            {
                if(universe.Status == UniverseStatus.Approved)
                {
                    ExistingUniversesList.Add(new ExistingUniversesData
                    {
                        UniverseID = universe.UniverseID,
                        UniverseName = universe.Name,
                        Existing = teamUniverses.Contains(universe.UniverseID)
                    });
                }
            }
        }

        public void UpdateTeamUniverses(string[] selectedUniverses, Team team)
        {
            if (selectedUniverses == null)
            {
                team.UniverseEntries = new List<UniverseEntry>();
                return;
            }

            var teamUniverses = new HashSet<int>(team.UniverseEntries.Select(c => c.Universe.UniverseID));
            foreach (var universe in Context.Universes.Include(x => x.UniverseEntries).ThenInclude(x => x.Team))
            {
                if (selectedUniverses.Contains(universe.UniverseID.ToString()))
                {
                    if (!teamUniverses.Contains(universe.UniverseID))
                    {
                        Context.UniverseEntries.Add(new UniverseEntry
                        {
                            UniverseID = universe.UniverseID,
                            TeamID = team.TeamID,
                            NewMember = NewMember.Team
                        });
                    }
                }
                else
                {
                    if (teamUniverses.Contains(universe.UniverseID))
                    {
                        UniverseEntry universeToRemove = team.UniverseEntries
                                                         .FirstOrDefault(i => i.UniverseID == universe.UniverseID);
                        Context.UniverseEntries.Remove(universeToRemove);
                    }
                }
            }
        }
    }
}