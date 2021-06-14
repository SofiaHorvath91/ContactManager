using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using ContactManager.Authorization;

namespace ContactManager.Pages.Favourites
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

        public int FavID;
        public FavType FavType;

        public IActionResult OnGet(string idtype)
        {
            var idtypeSplit = idtype.Split("_");
            FavID = Convert.ToInt32(idtypeSplit[0]);
            FavType = (FavType)Enum.Parse(typeof(FavType), idtypeSplit[1], true);

            return Page();
        }

        [BindProperty]
        public Favourite Favourite { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string favid, string favtype)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Favourite.OwnerID = UserManager.GetUserId(User);
            Favourite.SelectedFavID = Convert.ToInt32(favid);
            Favourite.FavType = (FavType)Enum.Parse(typeof(FavType), favtype, true);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Favourite,
                                                        ModelsOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Favourites.Add(Favourite);
            await Context.SaveChangesAsync();

            var path = "/" + Favourite.FavType.ToString() + "s/Details";
            return RedirectToPage(path, new { id = Favourite.SelectedFavID });
        }
    }
}
