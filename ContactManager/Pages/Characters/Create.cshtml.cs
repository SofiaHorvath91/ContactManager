using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ContactManager.Pages.Characters
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

        public List<ExistingPowersData> ExistingPowersList;
        public List<ExistingUniversesData> ExistingUniversesList;

        public IActionResult OnGet()
        {
            var character = new Character();
            character.CharacterPowerEntries = new List<CharacterPowerEntry>();
            PopulateExistingPowersUniversesTeams();

            return Page();
        }

        [BindProperty]
        public Character Character { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string[] selectedPowers, string[] selectedPowersSpecifications, string[] selectedPowersLevel, 
                                                     string[] selectedUniverses, string[] selectedTeams, string[] selectedTeamRole)
        {
            if (!ModelState.IsValid)
            {
                PopulateExistingPowersUniversesTeams();
                return Page();
            }

            Character.OwnerID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Character,
                                                        ModelsOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            using (var memoryStream = new MemoryStream())
            {
                if(FileUpload?.Length > 0)
                {
                    await FileUpload.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 524288)
                    {
                        Character.ProfileImage = memoryStream.ToArray();
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
                    Character.ProfileImage = System.IO.File.ReadAllBytes(path);
                    await Context.SaveChangesAsync();
                }
            }

            Context.Characters.Add(Character);
            await Context.SaveChangesAsync();

            for (var i = 0; i < selectedPowers.Length; i++)
            {
                var powerID = int.Parse(selectedPowers[i]);
                var index = powerID - 1;
                var foundPower = await Context.Powers.FindAsync(powerID);
                if (foundPower != null)
                {
                    Context.CharacterPowerEntries.Add(new CharacterPowerEntry
                    {
                        CharacterID = Character.CharacterID,
                        PowerID = foundPower.PowerID,
                        Specification = selectedPowersSpecifications[index] != null ? selectedPowersSpecifications[index] : null,
                        Level = selectedPowersLevel[index] != null ? (Level)Enum.Parse(typeof(Level), selectedPowersLevel[index], true) : null
                    });
                }
            }

            for(var i = 0; i < selectedUniverses.Length; i++)
            {
                var foundUniverse = await Context.Universes.FindAsync(int.Parse(selectedUniverses[i]));
                var universeTeams = Context.UniverseEntries.Where(x => x.Universe == foundUniverse).Select(x => x.Team.TeamName).ToList();
                if(foundUniverse != null && ! universeTeams.Any(x => selectedTeams.Contains(x)))
                {
                    Context.UniverseEntries.Add(new UniverseEntry { 
                        UniverseID = foundUniverse.UniverseID,
                        CharacterID = Character.CharacterID,
                        NewMember = NewMember.Character
                    });
                }
            }

            for (var i = 0; i < selectedTeams.Length; i++)
            {
                var teamID = Context.Teams.FirstOrDefault(t => t.TeamName == selectedTeams[i]).TeamID;
                var foundTeam = await Context.Teams.FindAsync(teamID);
                if (foundTeam != null)
                {
                    Context.TeamEntries.Add(new TeamEntry
                    {
                        TeamID = foundTeam.TeamID,
                        CharacterID = Character.CharacterID,
                        Role = selectedTeamRole[i] != null ? (Role)Enum.Parse(typeof(Role), selectedTeamRole[i], true) : Role.Member
                    });
                }
            }

            await Context.SaveChangesAsync();
            return RedirectToPage("./Details", new { id = Character.CharacterID });
        }

        private void PopulateExistingPowersUniversesTeams()
        {
            var allPowers = Context.Powers.Where(x=>x.Status == PowerStatus.Approved).Select(x=>x);
            ExistingPowersList = new List<ExistingPowersData>();
            foreach (var power in allPowers)
            {
                if(power.Status == PowerStatus.Approved)
                {
                    ExistingPowersList.Add(new ExistingPowersData
                    {
                        PowerID = power.PowerID,
                        PowerName = power.PowerName,
                    });
                }
            }

            var allUniverses = Context.Universes.Where(x => x.Status == UniverseStatus.Approved).Select(x => x);
            ExistingUniversesList = new List<ExistingUniversesData>();
            foreach (var universe in allUniverses)
            {
                var list = new List<string>();
                if(Context.UniverseEntries.Any(x => x.UniverseID == universe.UniverseID))
                {
                    list = Context.UniverseEntries
                           .Where(x => x.UniverseID == universe.UniverseID)
                           .Select(x=>x)
                           .Where(x=>x.Team.Status == TeamStatus.Approved)
                           .Select(x => x.Team.TeamName).Distinct().ToList();
                }

                ExistingUniversesList.Add(new ExistingUniversesData
                {
                    UniverseID = universe.UniverseID,
                    UniverseName = universe.Name,
                    Teams = list,
                    Existing = false
                });
            }
        }
    }
}
