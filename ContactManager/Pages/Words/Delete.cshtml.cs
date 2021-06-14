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

namespace ContactManager.Pages.Words
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
        public Word Word { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Word = await Context.Words.FirstOrDefaultAsync(m => m.WordID == id);

            if (Word == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                         User, Word,
                                         ModelsOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Word = await Context.Words.FindAsync(id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                         User, Word,
                                         ModelsOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            if (Word != null)
            {
                Context.Words.Remove(Word);
                await Context.SaveChangesAsync();
            }

            return RedirectToPage("/YourWorld");
        }
    }
}
