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

namespace ContactManager.Pages.Words
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

        public List<SelectListItem> WordsFandoms { get; set; }

        public IActionResult OnGet()
        {
            List<string> universeNames = Context.Universes.Select(x => x.Name).ToList();
            WordsFandoms = new List<SelectListItem>();
            for (int i = 0; i < universeNames.Count; i++)
            {
                SelectListItem newItem = new SelectListItem() {
                    Text = universeNames[i].ToString(),
                };
                WordsFandoms.Add(newItem);
            }

            return Page();
        }

        [BindProperty]
        public Word Word { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string[] fandomName)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Word.OwnerID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Word,
                                                        ModelsOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            if(fandomName.Length == 0)
            {
                Word.Fandom = null;
            }
            else
            {
                Word.Fandom = Context.Universes.Single(x => x.Name == fandomName[0]);
            }

            Context.Words.Add(Word);
            await Context.SaveChangesAsync();

            return RedirectToPage("/Games/Index");
        }
    }
}
