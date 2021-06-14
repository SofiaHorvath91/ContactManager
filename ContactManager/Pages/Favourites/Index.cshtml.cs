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

namespace ContactManager.Pages.Favourites
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

        public IList<Favourite> Favourite { get;set; }

        public Universe FavUniverse { get; set; }
        public Team FavTeam { get; set; }
        public Character FavCharacter { get; set; }

        public List<Universe> UserFavouriteUniverses { get; set; }
        public List<Team> UserFavouriteTeams { get; set; }
        public List<Character> UserFavouriteCharacters { get; set; }

        public async Task OnGetAsync()
        {
            string user = UserManager.GetUserId(User);

            Favourite = await Context.Favourites.ToListAsync();

            var maxFavUniverseID = GetMaxElement(FavType.Universe);
            var maxFavTeamID = GetMaxElement(FavType.Team);
            var maxFavCharacterID = GetMaxElement(FavType.Character);

            FavUniverse = Context.Universes.SingleOrDefault(x => x.UniverseID == maxFavUniverseID) != null ? 
                          Context.Universes.SingleOrDefault(x => x.UniverseID == maxFavUniverseID) : null;
            FavTeam = Context.Teams.SingleOrDefault(x => x.TeamID == maxFavTeamID) != null ?
                      Context.Teams.SingleOrDefault(x => x.TeamID == maxFavTeamID) : null;
            FavCharacter = Context.Characters.SingleOrDefault(x => x.CharacterID == maxFavCharacterID) != null ?
                           Context.Characters.SingleOrDefault(x => x.CharacterID == maxFavCharacterID) : null;

            var favUniversesIndexes = GetFavIndexesList(user, FavType.Universe);
            var favTeamsIndexes = GetFavIndexesList(user, FavType.Team);
            var favCharactersIndexes = GetFavIndexesList(user, FavType.Character);

            UserFavouriteUniverses = new List<Universe>();
            for (int i = 0; i < favUniversesIndexes.Count; i++)
            {
                UserFavouriteUniverses.Add(Context.Universes.SingleOrDefault(x => x.UniverseID == favUniversesIndexes[i]));
            }

            UserFavouriteTeams = new List<Team>();
            for (int i = 0; i < favTeamsIndexes.Count; i++)
            {
                UserFavouriteTeams.Add(Context.Teams.SingleOrDefault(x => x.TeamID == favTeamsIndexes[i]));
            }

            UserFavouriteCharacters = new List<Character>();
            for (int i = 0; i < favCharactersIndexes.Count; i++)
            {
                UserFavouriteCharacters.Add(Context.Characters.SingleOrDefault(x => x.CharacterID == favCharactersIndexes[i]));
            }
        }

        public int GetMaxElement(FavType type)
        {
            var favsByType = Favourite.Where(x => x.FavType == type).ToArray();
            if(favsByType.Length > 0)
            {
                var favsByTypeCount = favsByType.Select(x => favsByType.Count(y => y.SelectedFavID == x.SelectedFavID)).ToArray();
                var maxCount = favsByTypeCount.Max(x => x);
                var indexMax = Array.IndexOf(favsByTypeCount, maxCount);
            
                return favsByType[0].SelectedFavID;
            }
            else
            {
                return -1;
            }
        }

        public List<int> GetFavIndexesList(string user, FavType type)
        {
            return Favourite.Where(x => x.OwnerID == user && x.FavType == type).Select(x => x.SelectedFavID).ToList();
        }
    }
}
