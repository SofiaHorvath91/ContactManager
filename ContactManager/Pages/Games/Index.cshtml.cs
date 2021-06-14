using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace ContactManager.Pages.Games
{
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public List<SelectListItem> FandomsWithWords { get; set; }
        public List<SelectListItem> FandomsWithOpponents { get; set; }

        public IActionResult OnGet()
        {
            PopulateUniversesWithOpponents();
            PopulateUniversesWithWords();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string gameType, string universeNeeded, string[]? fandomName)
        {
            if(universeNeeded == "yes" && fandomName == null)
            {
                PopulateUniversesWithOpponents();
                PopulateUniversesWithWords();
                return Page();
            }

            if(universeNeeded == "yes")
            {
                if(Context.Universes.Any(x => x.Name == fandomName[0]))
                {
                    var universeID = Context.Universes.Single(x => x.Name == fandomName[0]).UniverseID;
                    var path = "/Games/" + gameType;
                    return RedirectToPage(path, new { id = universeID });
                }
                else
                {
                    PopulateUniversesWithWords();
                    PopulateUniversesWithOpponents();
                    return Page();
                }
            }
            else
            {
                var path = "/Games/" + gameType;
                return RedirectToPage(path);
            }
        }

        public void PopulateUniversesWithWords()
        {
            List<int> universesWwords = Context.Words.Where(x => x.Status == Models.WordStatus.Approved)
                                        .Select(x => x.Fandom.UniverseID).ToList();
            List<string> universeNames = Context.Universes.Where(x => universesWwords.Contains(x.UniverseID))
                                        .Select(x => x.Name).ToList();
            FandomsWithWords = new List<SelectListItem>();
            for (int i = 0; i < universeNames.Count; i++)
            {
                SelectListItem newItem = new SelectListItem()
                {
                    Text = universeNames[i].ToString(),
                };
                FandomsWithWords.Add(newItem);
            }
        }

        public void PopulateUniversesWithOpponents()
        {
            List<int> universesWopponents = Context.OpponentEntries.Where(x => x.Status == Models.OpponentStatus.Approved)
                                            .Select(x => x.OpponentsUniverse.UniverseID).ToList();
            List<string> universeNames = Context.Universes.Where(x => universesWopponents.Contains(x.UniverseID))
                                         .Select(x => x.Name).ToList();
            FandomsWithOpponents = new List<SelectListItem>();
            for (int i = 0; i < universeNames.Count; i++)
            {
                SelectListItem newItem = new SelectListItem()
                {
                    Text = universeNames[i].ToString(),
                };
                FandomsWithOpponents.Add(newItem);
            }
        }
    }
}
