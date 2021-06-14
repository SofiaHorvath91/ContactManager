using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ContactManager.Pages.Universes
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        [BindProperty]
        public Universe Universe { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Universe = await Context.Universes.FirstOrDefaultAsync(
                                                 m => m.UniverseID == id);

            if (Universe == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                      User, Universe,
                                                      ModelsOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var universe = await Context
                .Universes.AsNoTracking()
                .FirstOrDefaultAsync(m => m.UniverseID == id);

            if (universe == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, universe,
                                                     ModelsOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Universe.OwnerID = universe.OwnerID;

            Context.Attach(Universe).State = EntityState.Modified;

            if (Universe.Status == UniverseStatus.Approved)
            {
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        Universe,
                                        ModelsOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    Universe.Status = UniverseStatus.Submitted;
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                if (FileUpload?.Length > 0)
                {
                    await FileUpload.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 524288)
                    {
                        Universe.ProfileImage = memoryStream.ToArray();
                        await Context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                        return Page();
                    }
                }
                else
                {
                    var path = Path.Combine(Environment.WebRootPath, "img", $"profilepic.png");
                    universe.ProfileImage = Universe.ProfileImage.Length != 0 ? Universe.ProfileImage : System.IO.File.ReadAllBytes(path);
                    await Context.SaveChangesAsync();
                }
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("/Universes/Details", new { id = Universe.UniverseID });
        }
    }
}
