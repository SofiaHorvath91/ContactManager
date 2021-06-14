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

namespace ContactManager.Pages.Characters
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

        public Character Character { get; set; }

        public bool IsFavourite { get; set; }

        public int FavID { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Character = await Context.Characters
                      .Include(s => s.CharacterPowerEntries)
                          .ThenInclude(e => e.Power)
                      .Include(s => s.TeamEntries)
                          .ThenInclude(e => e.Team)
                            .ThenInclude(e => e.UniverseEntries)
                                .ThenInclude(e => e.Universe)
                      .Include(s => s.OpponentEntries)
                          .ThenInclude(e => e.Team)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Universe)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Team)
                      .FirstOrDefaultAsync(m => m.CharacterID == id);

            if (Character == null)
            {
                return NotFound();
            }

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != Character.OwnerID
                && Character.Status != CharacterStatus.Approved)
            {
                return Forbid();
            }

            if(Context.Favourites.Any(x => x.OwnerID == currentUserId &&
               x.SelectedFavID == Character.CharacterID && x.FavType == FavType.Character))
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
                        x.SelectedFavID == Character.CharacterID && x.FavType == FavType.Character).FavouriteID;
            }

            if (Character.ProfileImage == null || Character.ProfileImage.Length == 0)
            {
                var path = Path.Combine(Environment.WebRootPath, "img", $"profilepic.png");
                Character.ProfileImage = System.IO.File.ReadAllBytes(path);
                await Context.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, CharacterStatus status)
        {
            var character = await Context.Characters
                      .Include(s => s.CharacterPowerEntries)
                          .ThenInclude(e => e.Power)
                      .Include(s => s.TeamEntries)
                          .ThenInclude(e => e.Team)
                            .ThenInclude(e => e.UniverseEntries)
                                .ThenInclude(e => e.Universe)
                      .Include(s => s.OpponentEntries)
                          .ThenInclude(e => e.Team)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Universe)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Team)
                      .FirstOrDefaultAsync(m => m.CharacterID == id);

            if (character == null)
            {
                return NotFound();
            }

            var characterOperation = (status == CharacterStatus.Approved)
                                                       ? ModelsOperations.Approve
                                                       : ModelsOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, character,
                                        characterOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            character.Status = status;
            Context.Characters.Update(character);
            await Context.SaveChangesAsync();

            var redirectID = character.UniverseEntries.Count != 0 ? character.UniverseEntries.First().Universe.UniverseID
                             : Context.TeamEntries.First(x => x.CharacterID == character.CharacterID).Team.UniverseEntries.First().UniverseID;
            
            if(character.TeamEntries.Count > 0 || character.UniverseEntries.Count > 0)
            {
                return RedirectToPage("/Universes/Details", new { id = redirectID });
            }
            else
            {
                return RedirectToPage("/Universes/Index");
            }
        }
    }
}
