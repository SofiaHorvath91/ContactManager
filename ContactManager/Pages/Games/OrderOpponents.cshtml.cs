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

namespace ContactManager.Pages.Games
{
    public class OrderOpponentsModel : DI_BasePageModel
    {
        public OrderOpponentsModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public string OpponentsSideOne { get; set; }
        public string OpponentsSideTwo { get; set; }
        public List<OpponentEntry> Opponents { get; set; }

        public void OnGet(int? id)
        {
            Opponents = Context.OpponentEntries.Where(x => x.OpponentsUniverse.UniverseID == id && x.Status == OpponentStatus.Approved)
                        .Select(x => x).ToList();
            string sideOne = "";
            string sideTwo = "";

            for (int i = 0; i < Opponents.Count; i++)
            {
                if(Opponents[i].CharacterID != null)
                {
                    var character = Context.Characters.Single(x => x.CharacterID == Opponents[i].CharacterID);
                    sideOne += character.RealName + " / " + character.Alias + "_";
                }
                if (Opponents[i].TeamID != null)
                {
                    sideOne += Context.Teams.Single(x => x.TeamID == Opponents[i].TeamID).TeamName + "_";
                }
                if (Opponents[i].Item != null)
                {
                    sideOne += Opponents[i].Item + "_";
                }

                if (Opponents[i].OpponentCharacterID != null)
                {
                    var character = Context.Characters.Single(x => x.CharacterID == Opponents[i].OpponentCharacterID);
                    sideTwo += character.RealName + " / " + character.Alias + "_";
                }
                if (Opponents[i].OpponentTeamID != null)
                {
                    sideTwo += Context.Teams.Single(x => x.TeamID == Opponents[i].OpponentTeamID).TeamName + "_";
                }
                if (Opponents[i].OpponentItem != null)
                {
                    sideTwo += Opponents[i].OpponentItem + "_";
                }
            }

            OpponentsSideOne = sideOne;
            OpponentsSideTwo = sideTwo;
        }
    }
}
