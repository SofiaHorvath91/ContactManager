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
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Routing;

namespace ContactManager.Pages.Characters
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
        public Character Character { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        public List<ExistingPowersData> ExistingPowersList;
        public List<ExistingTeamsData> ExistingTeamsList;
        public List<ExistingUniversesData> ExistingUniversesList;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Character = await Context.Characters
                      .Include(s => s.CharacterPowerEntries)
                          .ThenInclude(e => e.Power)
                      .Include(s => s.TeamEntries)
                          .ThenInclude(e => e.Team)
                            .ThenInclude(e => e.UniverseEntries)
                      .Include(s => s.OpponentEntries)
                          .ThenInclude(e => e.Team)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Universe)
                      .Include(s => s.UniverseEntries)
                          .ThenInclude(e => e.Team)
                      .AsNoTracking()
                      .FirstOrDefaultAsync(m => m.CharacterID == id);

            if (Character == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                      User, Character,
                                                      ModelsOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            PopulateExistingPowers(Character);
            PopulateExistingUniverses(Character);
            PopulateExistingTeams(Character);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, 
                                                     string[] selectedPowers, string[] selectedPowersSpecifications, string[] selectedPowersLevel, 
                                                     string[] selectedUniverses, string[] selectedTeams, string[] selectedTeamRole)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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
                      .FirstOrDefaultAsync(m => m.CharacterID == id);

            if (character == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, character,
                                                     ModelsOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Character.OwnerID = character.OwnerID;
            Context.Entry(character).State = EntityState.Detached;
            Context.Attach(Character).State = EntityState.Modified;

            if (Character.Status == CharacterStatus.Approved)
            {
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        Character,
                                        ModelsOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    Character.Status = CharacterStatus.Submitted;
                }
            }

            Context.Entry(Character).State = EntityState.Detached;
            Context.Entry(character).State = EntityState.Modified;

            using (var memoryStream = new MemoryStream())
            {
                if (FileUpload?.Length > 0)
                {
                    await FileUpload.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 524288)
                    {
                        character.ProfileImage = memoryStream.ToArray();
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
                    character.ProfileImage = character.ProfileImage.Length != 0 ? character.ProfileImage : System.IO.File.ReadAllBytes(path);
                    await Context.SaveChangesAsync();
                }
            }

            if (await TryUpdateModelAsync<Character>(
                character,
                "Character",
                s => s.RealName, s => s.Alias, s => s.Introduction, s => s.Moral, s => s.Kind))
            {
                UpdateCharacterPowers(selectedPowers, selectedPowersSpecifications, selectedPowersLevel, character);
                UpdateCharacterUniversesTeams(selectedUniverses, selectedTeams, selectedTeamRole, character);
                await Context.SaveChangesAsync();
                return RedirectToPage("./Details", new { id = character.CharacterID });
            }

            UpdateCharacterPowers(selectedPowers, selectedPowersSpecifications, selectedPowersLevel, character);
            UpdateCharacterUniversesTeams(selectedUniverses, selectedTeams, selectedTeamRole, character);
            PopulateExistingPowers(character);
            PopulateExistingUniverses(character);
            PopulateExistingTeams(character);
            return Page();
        }

        private void PopulateExistingPowers(Character character)
        {
            var allPowers = Context.Powers.Where(x=>x.Status == PowerStatus.Approved).ToList();
            var characterPowers = new HashSet<int>(character.CharacterPowerEntries.Select(c => c.PowerID));
            ExistingPowersList = new List<ExistingPowersData>();
            foreach (var power in allPowers)
            {
                Level? level = null;
                string spec = null;
                var list = Enum.GetValues(typeof(Level)).Cast<Level>();
                if (character.CharacterPowerEntries.Any(c => c.PowerID == power.PowerID))
                {
                    level = (Level?)character.CharacterPowerEntries.Where(c => c.PowerID == power.PowerID).Single().Level;
                    spec = character.CharacterPowerEntries.Single(c => c.PowerID == power.PowerID).Specification;
                    list = Enum.GetValues(typeof(Level)).Cast<Level>().Where(e => e != level).Select(e => e).ToList();
                }

                ExistingPowersList.Add(new ExistingPowersData
                {
                    PowerID = power.PowerID,
                    PowerName = power.PowerName,
                    PowerDescription = spec,
                    Level = level,
                    Levels = new SelectList(list),
                    Existing = characterPowers.Contains(power.PowerID)
                });
            }
        }

        private void PopulateExistingUniverses(Character character)
        {
            var allUniverses = Context.Universes
                              .Where(x=>x.Status == UniverseStatus.Approved)
                              .Select(x=>x)
                              .Include(s => s.UniverseEntries)
                                  .ThenInclude(e => e.Character)
                              .Include(s => s.UniverseEntries)
                                  .ThenInclude(e => e.Team).ToList();
            var characterUniverses = new HashSet<int>(character.UniverseEntries.Select(c => c.UniverseID));
            var characterTeams = new HashSet<int>(character.TeamEntries.Select(c => c.TeamID));
            ExistingUniversesList = new List<ExistingUniversesData>();
            foreach (var universe in allUniverses)
            {
                var list = Context.UniverseEntries
                        .Where(x => x.UniverseID == universe.UniverseID)
                        .Select(x => x.Team.TeamName).Distinct().ToList() != null ?
                        Context.UniverseEntries
                        .Where(x => x.UniverseID == universe.UniverseID)
                        .Select(x => x.Team.TeamName).Distinct().ToList() : new List<string>();

                List<int> universeTeams = universe.UniverseEntries.Where(x => x.TeamID != null)
                                          .ToList().Select(x => (int)x.TeamID).ToList();

                ExistingUniversesList.Add(new ExistingUniversesData
                {
                    UniverseID = universe.UniverseID,
                    UniverseName = universe.Name,
                    Teams = list,
                    Existing = characterUniverses.Contains(universe.UniverseID) || 
                               universeTeams.Intersect(characterTeams).Count() > 0 ? true : false
                });
            }
        }

        private void PopulateExistingTeams(Character character)
        {
            var allTeams = Context.Teams
                          .Where(x => x.Status == TeamStatus.Approved)
                          .Select(x => x)
                          .Include(s => s.TeamEntries)
                               .ThenInclude(e => e.Team);
            ExistingTeamsList = new List<ExistingTeamsData>();
            foreach (var team in allTeams)
            {
                Role? role = null;
                bool exist = false;
                var list = Enum.GetValues(typeof(Role)).Cast<Role>();
                if (ExistingUniversesList.Any(x => x.Teams.Contains(team.TeamName) && x.Existing == true)
                    && character.TeamEntries.Any(x => x.TeamID == team.TeamID))
                {
                    exist = true;
                    role = (Role?)character.TeamEntries.Single(c => c.TeamID == team.TeamID).Role;
                    list = Enum.GetValues(typeof(Role)).Cast<Role>().Where(e => e != role).Select(e => e).ToList();
                }

                ExistingTeamsList.Add(new ExistingTeamsData
                {
                    TeamID = team.TeamID,
                    TeamName = team.TeamName,
                    Role = role,
                    Roles = new SelectList(list),
                    Existing = exist
                });
            }
        }

        public void UpdateCharacterPowers(string[] selectedPowers, string[] selectedPowersSpecifications, 
                                          string[] selectedPowersLevel, Character character)
        {
            if (selectedPowers == null)
            {
                character.CharacterPowerEntries = new List<CharacterPowerEntry>();
                return;
            }

            var selectedPowersHS = new HashSet<string>(selectedPowers);
            var characterPowers = new HashSet<int>
                (character.CharacterPowerEntries.Select(c => c.Power.PowerID));
            var powers = Context.Powers.ToList();
            foreach (var power in powers)
            {
                var i = power.PowerID - 1;
                if (selectedPowersHS.Contains(power.PowerID.ToString())) {
                    if (!characterPowers.Contains(power.PowerID)) {
                        Context.CharacterPowerEntries.Add(new CharacterPowerEntry
                        {
                            CharacterID = character.CharacterID,
                            PowerID = power.PowerID,
                            Specification = selectedPowersSpecifications[i],
                            Level = (Level)Enum.Parse(typeof(Level), selectedPowersLevel[i], true)
                        });
                    }
                    else {
                        var powerEntries = Context.CharacterPowerEntries.ToList();
                        var powerEntry= powerEntries.SingleOrDefault(p => p.PowerID == power.PowerID
                                        && p.CharacterID == character.CharacterID);
                        powerEntry.Specification = selectedPowersSpecifications[i];
                        powerEntry.Level = selectedPowersLevel[i] != null ?
                            (Level)Enum.Parse(typeof(Level), selectedPowersLevel[i], true) : powerEntry.Level;
                    }
                }
                else {

                    if (characterPowers.Contains(power.PowerID)){
                        CharacterPowerEntry powerToRemove = character.CharacterPowerEntries
                                                            .FirstOrDefault(i => i.PowerID == power.PowerID);
                        Context.CharacterPowerEntries.Remove(powerToRemove);
                    }
                }
            }
        }
    
        public void UpdateCharacterUniversesTeams(string[] selectedUniverses, string[] selectedTeams, 
                                                  string[] selectedTeamRole, Character character)
        {
            if (selectedUniverses == null)
            {
                character.UniverseEntries = new List<UniverseEntry>();
                return;
            }
            if (selectedTeams == null)
            {
                character.TeamEntries = new List<TeamEntry>();
                return;
            }

            var characterUniverses = new HashSet<int>(character.UniverseEntries.Select(c => c.Universe.UniverseID));
            var characterTeams = new HashSet<int>(character.TeamEntries.Select(c => c.Team.TeamID));
            foreach (var universe in Context.Universes.Include(x=>x.UniverseEntries).ThenInclude(x=>x.Team).ToList())
            {
                if (selectedUniverses.Contains(universe.UniverseID.ToString()))
                {
                    if (!characterUniverses.Contains(universe.UniverseID))
                    {
                        var universeTeams = universe.UniverseEntries.Where(x=>x.TeamID != null).Select(x => x.Team.TeamID.ToString()).ToList();
                        if(universeTeams.Intersect(selectedTeams).ToList().Count == 0)
                        {
                            Context.UniverseEntries.Add(new UniverseEntry
                            {
                                UniverseID = universe.UniverseID,
                                CharacterID = character.CharacterID,
                                NewMember = NewMember.Character
                            });
                        }
                        else
                        {
                            for (int i = 0; i < selectedTeams.Length; i++) {
                                int teamID = Convert.ToInt32(selectedTeams[i]);
                                var index = selectedTeams.ToList().IndexOf(selectedTeams[i].ToString());
                                TeamEntry entryToModify = Context.TeamEntries.Single(x =>
                                                            x.CharacterID == character.CharacterID
                                                            && x.TeamID == teamID);

                                if (!characterTeams.Contains(teamID)) {
                                    Context.TeamEntries.Add(new TeamEntry
                                    {
                                        TeamID = teamID,
                                        CharacterID = character.CharacterID,
                                        Role = selectedTeamRole[index] != null ?
                                               (Role)Enum.Parse(typeof(Role), selectedTeamRole[index], true) :
                                               entryToModify.Role
                                    });
                                }
                                else {
                                    entryToModify.Role = selectedTeamRole[index] != null ?
                                                         (Role)Enum.Parse(typeof(Role), selectedTeamRole[index], true) 
                                                         : entryToModify.Role;
                                }
                            }

                            var characterteamsList = characterTeams.ToList();
                            for (int i = 0; i < characterteamsList.Count; i++) {
                                if (!selectedTeams.Contains(characterteamsList[i].ToString())) {
                                    TeamEntry entryToRemove = Context.TeamEntries.Single(x =>
                                                              x.CharacterID == character.CharacterID
                                                              && x.TeamID == characterteamsList[i]);
                                    Context.TeamEntries.Remove(entryToRemove);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var teamsCharacter = Context.TeamEntries.Where(x => x.CharacterID == character.CharacterID).Select(x => x);

                    if (characterUniverses.Contains(universe.UniverseID) 
                        || teamsCharacter.Any(x=>x.Team.UniverseEntries.Any(y=>y.UniverseID == universe.UniverseID)))
                    {
                        UniverseEntry universeToRemove = character.UniverseEntries
                                                         .FirstOrDefault(i => i.UniverseID == universe.UniverseID);

                        List<TeamEntry> teams = teamsCharacter.Where(x => 
                                                x.Team.UniverseEntries.Any(y=>y.UniverseID == universe.UniverseID)).ToList();

                        if(teams.Count > 0) {
                            for (int i = 0; i < teams.Count; i++) {
                                Context.TeamEntries.Remove(teams[i]);
                            }
                        }

                        if(universeToRemove != null) {
                            Context.UniverseEntries.Remove(universeToRemove);
                        }
                    }
                }
            }
        }
    }
}