using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ContactManager.Pages.Teams
{
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public List<ExistingUniversesData> ExistingUniversesList;

        public IActionResult OnGet()
        {
            var team = new Team();
            team.UniverseEntries = new List<UniverseEntry>();
            PopulateExistingUniverses(team);
            return Page();
        }

        [BindProperty]
        public Team Team { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string[] selectedUniverses)
        {
            if (!ModelState.IsValid)
            {
                PopulateExistingUniverses(Team);
                return Page();
            }

            Team.OwnerID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Team,
                                                        ModelsOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            using (var memoryStream = new MemoryStream())
            {
                if (FileUpload?.Length > 0)
                {
                    await FileUpload.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 524288)
                    {
                        Team.ProfileImage = memoryStream.ToArray();
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
                    Team.ProfileImage = System.IO.File.ReadAllBytes(path);
                    await Context.SaveChangesAsync();
                }
            }

            Context.Teams.Add(Team);
            await Context.SaveChangesAsync();

            for (var i = 0; i < selectedUniverses.Length; i++)
            {
                var foundUniverse = await Context.Universes.FindAsync(int.Parse(selectedUniverses[i]));
                if (foundUniverse != null)
                {
                    Context.UniverseEntries.Add(new UniverseEntry
                    {
                        UniverseID = foundUniverse.UniverseID,
                        TeamID = Team.TeamID,
                        NewMember = NewMember.Team
                    });
                }
            }

            await Context.SaveChangesAsync();
            if (selectedUniverses.Length > 0)
            {
                return RedirectToPage("/Universes/Details", new { id = selectedUniverses[0] });
            }
            else
            {
                return RedirectToPage("/Universes/Index");
            }
        }

        private void PopulateExistingUniverses(Team team)
        {
            var allUniverses = Context.Universes;
            ExistingUniversesList = new List<ExistingUniversesData>();
            foreach (var universe in allUniverses)
            {
                if(universe.Status == UniverseStatus.Approved)
                {
                    ExistingUniversesList.Add(new ExistingUniversesData
                    {
                        UniverseID = universe.UniverseID,
                        UniverseName = universe.Name,
                        Existing = false
                    });
                }
            }
        }
    }
}
