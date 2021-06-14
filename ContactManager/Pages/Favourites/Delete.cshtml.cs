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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using ContactManager.Authorization;

namespace ContactManager.Pages.Favourites
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        [BindProperty]
        public Favourite Favourite { get; set; }

        public int FavID;
        public FavType FavType;

        public async Task<IActionResult> OnGetAsync(string idtype)
        {
            var id = Convert.ToInt32(idtype.Split("_")[0]);
            FavID = Convert.ToInt32(idtype.Split("_")[1]);
            FavType = (FavType)Enum.Parse(typeof(FavType), idtype.Split("_")[2], true);

            Favourite = await Context.Favourites.FirstOrDefaultAsync(m => m.FavouriteID == id);

            if (Favourite == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, Favourite,
                                                     ModelsOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string favid, string favtype)
        {
            var currentUserId = UserManager.GetUserId(User);
            FavType type = (FavType)Enum.Parse(typeof(FavType), favtype, true);

            if (! Context.Favourites.Any(x => x.OwnerID == currentUserId &&
                x.SelectedFavID == Convert.ToInt32(favid) && x.FavType == type))
            {
                return NotFound();
            }

            Favourite = await Context.Favourites.SingleAsync(x => x.OwnerID == currentUserId &&
                        x.SelectedFavID == Convert.ToInt32(favid) && x.FavType == type);

            if (Favourite != null)
            {
                Context.Favourites.Remove(Favourite);
                await Context.SaveChangesAsync();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, Favourite,
                                                     ModelsOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            var path = "/" + favtype + "s/Details";
            return RedirectToPage(path, new { id = Convert.ToInt32(favid) });
        }
    }
}
