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

namespace ContactManager.Pages.Opponents
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

        public Universe Universe { get; set; }
        public bool UniverseSelected { get; set; }

        public IActionResult OnGet()
        {
            ViewData["Universes"] = new SelectList(Context.Universes, "UniverseID", "Name");

            return Page();
        }

        [BindProperty]
        public OpponentEntry OpponentEntry { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string[]? selectedUniverse, string[]? chosenUniverse)
        {
            if (selectedUniverse.Length == 0)
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                OpponentEntry.OwnerID = UserManager.GetUserId(User);

                var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                       User, OpponentEntry,
                                                       ModelsOperations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                OpponentEntry.OpponentsUniverse = Context.Universes.Single(x => x.UniverseID == Convert.ToInt32(chosenUniverse[0]));
                Context.OpponentEntries.Add(OpponentEntry);
                await Context.SaveChangesAsync();

                return RedirectToPage("/Games/Index");
            }
            else
            {
                Universe = Context.Universes.Single(x => x.UniverseID == Convert.ToInt32(selectedUniverse[0]));

                int id = Context.Universes.Single(x => x.UniverseID == Universe.UniverseID).UniverseID;
                var teams = Context.Teams.Where(x => x.UniverseEntries.Any(x => x.UniverseID == id)).Select(x => x).ToList();
                var charactersFromTeams = Context.Characters.Where(x =>
                                 x.TeamEntries.Any(y =>
                                 y.Team.UniverseEntries.Any(z => z.UniverseID == id))).
                                 Select(x => x).ToList();
                var characterStandalone = Context.Characters.Where(x => 
                                          x.UniverseEntries.Any(y => 
                                          y.UniverseID == id))
                                          .Select(x=>x).ToList();
                var characters = charactersFromTeams.Union(characterStandalone);

                ViewData["CharacterID"] = new SelectList(characters, "CharacterID", "Alias");
                ViewData["TeamID"] = new SelectList(teams, "TeamID", "TeamName");
                ViewData["Universes"] = new SelectList(Context.Universes, "UniverseID", "Name");

                UniverseSelected = true;

                return Page();
            }
        }
    }
}
