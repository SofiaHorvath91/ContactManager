using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace ContactManager.Pages.Characters
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
        public Character Character { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Character = await Context.Characters.FirstOrDefaultAsync(
                                                 m => m.CharacterID == id);

            if (Character == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, Character,
                                                     ModelsOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var character = await Context.Characters
                              .Include(s => s.CharacterPowerEntries)
                                  .ThenInclude(e => e.Power)
                              .Include(s => s.TeamEntries)
                                  .ThenInclude(e => e.Team)
                              .Include(s => s.OpponentEntries)
                                  .ThenInclude(e => e.Team)
                              .Include(s => s.UniverseEntries)
                                  .ThenInclude(e => e.Universe)
                              .Include(s => s.UniverseEntries)
                                  .ThenInclude(e => e.Team)
                              .AsNoTracking()
                              .FirstOrDefaultAsync(m => m.CharacterID == id);

            if (character == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, character,
                                                     ModelsOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Characters.Remove(character);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = character.CharacterID });
        }
    }
}
