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
using Microsoft.Extensions.Configuration;

namespace ContactManager.Pages.Games
{
    public class HangmanGameModel : DI_BasePageModel
    {
        public HangmanGameModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public string Words { get; set; }

        public IActionResult OnGet(int? id)
        {
            List<string> words = Context.Words.Where(x => x.Fandom.UniverseID==id && x.Status == Models.WordStatus.Approved)
                                .Select(x=>x.WordString).ToList();
            string wordsString = "";
            for (int i = 0; i < words.Count; i++)
            {
                wordsString += words[i] + "_";
            }

            Words = wordsString;

            return Page();
        }
    }
}
