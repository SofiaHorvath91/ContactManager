using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace ContactManager.Pages
{
    public class YourWorldModel : DI_BasePageModel
    {
        public YourWorldModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public List<Universe> UserUniverses { get; set; }
        public List<Team> UserTeams { get; set; }
        public List<Character> UserCharacters { get; set; }
        public List<Power> UserPowers { get; set; }
        public List<Word> UserWords { get; set; }
        public List<string> UserOpponentPairs { get; set; }

        public void OnGet()
        {
            var currentUser = UserManager.GetUserId(User);

            UserUniverses = Context.Universes.Where(x => x.OwnerID == currentUser).Select(x => x).ToList();
            UserTeams = Context.Teams.Where(x => x.OwnerID == currentUser).Select(x => x).ToList();
            UserCharacters = Context.Characters.Where(x => x.OwnerID == currentUser).Select(x => x).ToList();
            UserPowers = Context.Powers.Where(x => x.OwnerID == currentUser).Select(x => x).ToList();
            UserWords = Context.Words.Where(x => x.OwnerID == currentUser).Select(x => x).ToList();
            var Opponents = Context.OpponentEntries.Where(x => x.OwnerID == currentUser).Select(x => x).ToList();

            UserOpponentPairs = new List<string>();
            for (int i = 0; i < Opponents.Count; i++)
            {
                string pair = "";
                if (Opponents[i].CharacterID != null)
                {
                    var character = Context.Characters.Single(x => x.CharacterID == Opponents[i].CharacterID);
                    pair += character.RealName + " / " + character.Alias + "_";
                }
                if (Opponents[i].TeamID != null)
                {
                    pair += Context.Teams.Single(x => x.TeamID == Opponents[i].TeamID).TeamName + "_";
                }
                if (Opponents[i].Item != null)
                {
                    pair += Opponents[i].Item + "_";
                }

                if (Opponents[i].OpponentCharacterID != null)
                {
                    var character = Context.Characters.Single(x => x.CharacterID == Opponents[i].OpponentCharacterID);
                    pair += character.RealName + " / " + character.Alias;
                }
                if (Opponents[i].OpponentTeamID != null)
                {
                    pair += Context.Teams.Single(x => x.TeamID == Opponents[i].OpponentTeamID).TeamName;
                }
                if (Opponents[i].OpponentItem != null)
                {
                    pair += Opponents[i].OpponentItem;
                }

                pair += "_" + Opponents[i].OpponentsUniverse.Name + "_" + Opponents[i].OpponentEntryID + "_" + Opponents[i].Status;

                UserOpponentPairs.Add(pair);
            }
        }
    }
}
