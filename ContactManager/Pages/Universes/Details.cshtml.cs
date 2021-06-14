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
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ContactManager.Pages.Universes
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

        public Universe Universe { get; set; }

        public bool IsFavourite { get; set; }

        public int FavID { get; set; }

        public int TotalTeamsCount { get; set; }

        public int TotalCharactersCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Universe = await Context.Universes
                        .Include(s => s.UniverseEntries)
                            .ThenInclude(e => e.Team)
                                .ThenInclude(e => e.TeamEntries)
                        .Include(s => s.UniverseEntries)
                            .ThenInclude(e => e.Character)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.UniverseID == id);

            if (Universe == null)
            {
                return NotFound();
            }

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != Universe.OwnerID
                && Universe.Status != UniverseStatus.Approved)
            {
                return Forbid();
            }

            if(Context.Favourites.Any(x => x.OwnerID == currentUserId &&
               x.SelectedFavID == Universe.UniverseID && x.FavType == FavType.Universe))
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
                        x.SelectedFavID == Universe.UniverseID && x.FavType == FavType.Universe).FavouriteID;
            }

            TotalTeamsCount = Universe.UniverseEntries.Where(x => x.NewMember == NewMember.Team).Count();

            var teamsCharacters = Universe.UniverseEntries
                                 .Where(x => x.NewMember == NewMember.Team)
                                 .Select(x => x.Team.TeamEntries.Count).Sum();

            TotalCharactersCount = (Universe.UniverseEntries.Where(x => x.NewMember == NewMember.Character).Count()) + teamsCharacters;

            if(Universe.ProfileImage == null || Universe.ProfileImage.Length == 0)
            {
                var path = Path.Combine(Environment.WebRootPath, "img", $"profilepic.png");
                Universe.ProfileImage = System.IO.File.ReadAllBytes(path);
                await Context.SaveChangesAsync();
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, UniverseStatus status)
        {
            var universe = await Context.Universes
                        .Include(s => s.UniverseEntries)
                            .ThenInclude(e => e.Team)
                        .Include(s => s.UniverseEntries)
                            .ThenInclude(e => e.Character)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.UniverseID == id);

            if (universe == null)
            {
                return NotFound();
            }

            var universeOperation = (status == UniverseStatus.Approved)
                                                       ? ModelsOperations.Approve
                                                       : ModelsOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, universe,
                                        universeOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            universe.Status = status;
            Context.Universes.Update(universe);
            await Context.SaveChangesAsync();

            return RedirectToPage("/Universes/Details", new { id = universe.UniverseID });
        }
    }
}
