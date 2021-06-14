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

namespace ContactManager.Pages.Opponents
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
        public OpponentEntry OpponentEntry { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OpponentEntry = await Context.OpponentEntries
                .Include(o => o.Character)
                .Include(o => o.Team).FirstOrDefaultAsync(m => m.OpponentEntryID == id);

            if (OpponentEntry == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                        User, OpponentEntry,
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

            OpponentEntry = await Context.OpponentEntries.FindAsync(id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                         User, OpponentEntry,
                                         ModelsOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            if (OpponentEntry != null)
            {
                Context.OpponentEntries.Remove(OpponentEntry);
                await Context.SaveChangesAsync();
            }

            return RedirectToPage("/YourWorld");
        }
    }
}
