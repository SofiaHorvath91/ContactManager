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

namespace ContactManager.Pages
{
    [AllowAnonymous]
    public class AboutModel : DI_BasePageModel
    {
        public AboutModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public int UniversesCount { get; set; }
        public int TeamsCount { get; set; }
        public int CharactersCount { get; set; }

        public void OnGet()
        {
            UniversesCount = Context.Universes.Count();
            TeamsCount = Context.Teams.Count();
            CharactersCount = Context.Characters.Count();   
        }
    }
}
