using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Authorization;
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
    public class AdminModel : DI_BasePageModel
    {
        public AdminModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public List<Character> SubmittedCharacters { get; set; }
        public List<Power> SubmittedPowers { get; set; }
        public List<Team> SubmittedTeams { get; set; }
        public List<Universe> SubmittedUniverses { get; set; }
        public List<Word> SubmittedWords { get; set; }
        public List<OpponentEntry> SubmittedOpponentEntries { get; set; }
        public List<string> SubmittedOpponents { get; set; }

        public void OnGet()
        {
            SubmittedCharacters = Context.Characters.Where(x => x.Status == CharacterStatus.Submitted).Select(x => x).ToList();
            SubmittedPowers = Context.Powers.Where(x => x.Status == PowerStatus.Submitted).Select(x => x).ToList();
            SubmittedTeams = Context.Teams.Where(x => x.Status == TeamStatus.Submitted).Select(x => x).ToList();
            SubmittedUniverses = Context.Universes.Where(x => x.Status == UniverseStatus.Submitted).Select(x => x).ToList();
            SubmittedWords = Context.Words.Where(x => x.Status == WordStatus.Submitted).Select(x => x).ToList();
            SubmittedOpponentEntries = Context.OpponentEntries.Where(x => x.Status == OpponentStatus.Submitted).Select(x => x).ToList();

            SubmittedOpponents = new List<string>();
            for (int i = 0; i < SubmittedOpponentEntries.Count; i++)
            {
                string pair = "";
                if (SubmittedOpponentEntries[i].CharacterID != null)
                {
                    var character = Context.Characters.Single(x => x.CharacterID == SubmittedOpponentEntries[i].CharacterID);
                    pair += character.RealName + " / " + character.Alias + "_";
                }
                if (SubmittedOpponentEntries[i].TeamID != null)
                {
                    pair += Context.Teams.Single(x => x.TeamID == SubmittedOpponentEntries[i].TeamID).TeamName + "_";
                }
                if (SubmittedOpponentEntries[i].Item != null)
                {
                    pair += SubmittedOpponentEntries[i].Item + "_";
                }

                if (SubmittedOpponentEntries[i].OpponentCharacterID != null)
                {
                    var character = Context.Characters.Single(x => x.CharacterID == SubmittedOpponentEntries[i].OpponentCharacterID);
                    pair += character.RealName + " / " + character.Alias;
                }
                if (SubmittedOpponentEntries[i].OpponentTeamID != null)
                {
                    pair += Context.Teams.Single(x => x.TeamID == SubmittedOpponentEntries[i].OpponentTeamID).TeamName;
                }
                if (SubmittedOpponentEntries[i].OpponentItem != null)
                {
                    pair += SubmittedOpponentEntries[i].OpponentItem;
                }

                pair += "_" + SubmittedOpponentEntries[i].OpponentEntryID;

                SubmittedOpponents.Add(pair);
            }
        }

        public async Task<IActionResult> OnPostAsync(int id, string status)
        {
            var type = status.Split("_")[0];
            var decision = status.Split("_")[1];
            var operation = (decision == "Approved") ? ModelsOperations.Approve : ModelsOperations.Reject;

            if(type == "Character")
            {
                var character = Context.Characters.Single(m => m.CharacterID == id);
                if (character == null){ return NotFound(); }

                var isAuthorized = await AuthorizationService.AuthorizeAsync(User, character, operation);
                if (!isAuthorized.Succeeded){ return Forbid(); }

                character.Status = (CharacterStatus)Enum.Parse(typeof(CharacterStatus), decision, true);
                Context.Characters.Update(character);
            }
            if (type == "Team")
            {
                var team = Context.Teams.Single(m => m.TeamID == id);
                if (team == null) { return NotFound(); }

                var isAuthorized = await AuthorizationService.AuthorizeAsync(User, team, operation);
                if (!isAuthorized.Succeeded) { return Forbid(); }

                team.Status = (TeamStatus)Enum.Parse(typeof(TeamStatus), decision, true);
                Context.Teams.Update(team);
            }
            if (type == "Universe")
            {
                var universe = Context.Universes.Single(m => m.UniverseID == id);
                if (universe == null) { return NotFound(); }

                var isAuthorized = await AuthorizationService.AuthorizeAsync(User, universe, operation);
                if (!isAuthorized.Succeeded) { return Forbid(); }

                universe.Status = (UniverseStatus)Enum.Parse(typeof(UniverseStatus), decision, true);
                Context.Universes.Update(universe);
            }
            if (type == "Power")
            {
                var power = Context.Powers.Single(m => m.PowerID == id);
                if (power == null) { return NotFound(); }

                var isAuthorized = await AuthorizationService.AuthorizeAsync(User, power, operation);
                if (!isAuthorized.Succeeded) { return Forbid(); }

                power.Status = (PowerStatus)Enum.Parse(typeof(PowerStatus), decision, true);
                Context.Powers.Update(power);
            }
            if (type == "Word")
            {
                var word = Context.Words.Single(m => m.WordID == id);
                if (word == null) { return NotFound(); }

                var isAuthorized = await AuthorizationService.AuthorizeAsync(User, word, operation);
                if (!isAuthorized.Succeeded) { return Forbid(); }

                word.Status = (WordStatus)Enum.Parse(typeof(WordStatus), decision, true);
                Context.Words.Update(word);
            }
            if (type == "Pair")
            {
                var pair = Context.OpponentEntries.Single(m => m.OpponentEntryID == id);
                if (pair == null) { return NotFound(); }

                var isAuthorized = await AuthorizationService.AuthorizeAsync(User, pair, operation);
                if (!isAuthorized.Succeeded) { return Forbid(); }

                pair.Status = (OpponentStatus)Enum.Parse(typeof(OpponentStatus), decision, true);
                Context.OpponentEntries.Update(pair);
            }

            await Context.SaveChangesAsync();
            return RedirectToPage("./Admin");
        }
    }
}
