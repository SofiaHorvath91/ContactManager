using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ContactManager.Pages.Teams
{
    public class DetailsModel : DI_BasePageModel
    {
        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public Team Team { get; set; }

        public bool IsFavourite { get; set; }

        public int FavID { get; set; }

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

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != Team.OwnerID
                && Team.Status != TeamStatus.Approved)
            {
                return Forbid();
            }

            if(Context.Favourites.Any(x => x.OwnerID == currentUserId &&
               x.SelectedFavID == Team.TeamID && x.FavType == FavType.Team))
            {
                IsFavourite = true;
            }
            else
            {
                IsFavourite = false;
            }

            if (IsFavourite)
            {
                FavID = Context.Favourites.Single(x => x.OwnerID == currentUserId &&
                        x.SelectedFavID == Team.TeamID && x.FavType == FavType.Team).FavouriteID;
            }

            if (Team.ProfileImage == null || Team.ProfileImage.Length == 0)
            {
                var path = Path.Combine(Environment.WebRootPath, "img", $"profilepic.png");
                Team.ProfileImage = System.IO.File.ReadAllBytes(path);
                await Context.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, TeamStatus status)
        {
            var team = await Context.Teams
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

            if (team == null)
            {
                return NotFound();
            }

            var teamOperation = (status == TeamStatus.Approved)
                                                       ? ModelsOperations.Approve
                                                       : ModelsOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, team,
                                        teamOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            team.Status = status;
            Context.Teams.Update(team);
            await Context.SaveChangesAsync();

            if (team.UniverseEntries.Count > 0)
            {
                return RedirectToPage("/Universes/Details", new { id = team.UniverseEntries.First().UniverseID });
            }
            else
            {
                return RedirectToPage("/Universes/Index");
            }
        }
    }
}
