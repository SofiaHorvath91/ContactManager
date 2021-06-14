using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System;

namespace ContactManager.Pages.Powers
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

        public int CharacterID { get; set; }

        public IActionResult OnGet(int? id)
        {
            CharacterID = (int)id;
            return Page();
        }

        [BindProperty]
        public Power Power { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string[] characterID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Power.OwnerID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Power,
                                                        ModelsOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Powers.Add(Power);
            await Context.SaveChangesAsync();

            return RedirectToPage("/Characters/Details", new { id = Convert.ToInt32(characterID[0]) });
        }
    }
}
