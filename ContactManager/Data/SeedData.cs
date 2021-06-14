using ContactManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;

namespace ContactManager.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "--PUT YOUR OWN DATA--");
                await EnsureRole(serviceProvider, adminID, Constants.ContactAdministratorsRole);

                // allowed user can create and edit contacts that they create
                var managerID = await EnsureUser(serviceProvider, testUserPw, "--PUT YOUR OWN DATA--");
                await EnsureRole(serviceProvider, managerID, Constants.ContactManagersRole);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Universes.Any())
            {
                return;   // DB has been seeded
            }

            var universes = new Universe[] {
                new Universe{Name="MCU",Creator="Stan Lee",OwnerID = adminID, ProfileImage = new byte[]{},
                             Origin=Origin.Comic,Genre=Genre.Superhero,Status=UniverseStatus.Approved},
                new Universe{Name="DCU",Creator="Malcolm Wheeler-Nicholson",OwnerID = adminID, ProfileImage = new byte[]{},
                             Origin=Origin.Comic,Genre=Genre.Superhero,Status=UniverseStatus.Approved}
            };
            foreach (Universe u in universes)
            {
                context.Universes.Add(u);
            }
            context.SaveChanges();

            var characters = new Character[] {
                new Character{RealName="Tony Stark",Alias="Ironman", OwnerID = adminID, ProfileImage = new byte[]{ },
                    Introduction="Genius, billionaire, playboy, philanthropist - and also a superhero.",
                    Moral=Moral.Good, Kind=Kind.Human,Status=CharacterStatus.Approved},
                new Character{RealName="Bruce Banner",Alias="Hulk", OwnerID = adminID, ProfileImage = new byte[]{ },
                    Introduction="Scientist and physicist, famed for his work into the studies of nuclear physics and gamma radiation",
                    Moral=Moral.Good, Kind=Kind.Mutant,Status=CharacterStatus.Approved},
                new Character{RealName="Clark Kent",Alias="Superman", OwnerID = adminID, ProfileImage = new byte[]{ },
                    Introduction="Last son of Krypton, sent as the dying planet's last hope to Earth, where he grew to become its protector.",
                    Moral=Moral.Good, Kind=Kind.Alien,Status=CharacterStatus.Approved},
                new Character{RealName="Thanos",Alias="Mad Titan", OwnerID = adminID, ProfileImage = new byte[]{ },
                    Introduction="Genocidal warlord from Titan, whose own main objective was to bring stability to the universe by wiping out half of all life at every level.",
                    Moral=Moral.Bad, Kind=Kind.Alien,Status=CharacterStatus.Approved}, 
                new Character{RealName="Johann Schmidt",Alias="Red Skull", OwnerID = adminID, ProfileImage = new byte[]{ },
                    Introduction="Former head of HYDRA, the special weapons division of the Nazi Schutzstaffel who gained superhuman abilites and monstrous look by taking the early form of the Erskine-serum.",
                    Moral=Moral.Bad, Kind=Kind.Mutant,Status=CharacterStatus.Approved},
                new Character{RealName="Alexander Luthor",Alias="Lex Luthor", OwnerID = adminID, ProfileImage = new byte[]{ },
                    Introduction="Genius, ego-centered Metropolis businessman who, with his company LexCorp, has tired to destory Superman and the Justice League.",
                    Moral=Moral.Bad, Kind=Kind.Human,Status=CharacterStatus.Approved},
                new Character{RealName="Jack Napier",Alias="The Joker", OwnerID = adminID, ProfileImage = new byte[]{ },
                    Introduction="Criminal mastermind and psychopath with a warped, sadistic sense of humor who aims uncontrolled chaos and destruction.",
                    Moral=Moral.Bad, Kind=Kind.Human,Status=CharacterStatus.Approved}
            };
            foreach (Character c in characters)
            {
                context.Characters.Add(c);
            }
            context.SaveChanges();

            var powers = new Power[]
            {
                new Power{PowerName="Super Strength",PowerDescription="Superhuman physical strength",
                    OwnerID = adminID,PowerType=PowerType.Superhuman,Status=PowerStatus.Approved},
                new Power{PowerName="Intelligence",PowerDescription="High-level intelligence from nature or magic",
                    OwnerID = adminID,PowerType=PowerType.Both,Status=PowerStatus.Approved},
                new Power{PowerName="Flying",PowerDescription="Being able to fly without plane by techology or magic",
                    OwnerID = adminID,PowerType=PowerType.Both,Status=PowerStatus.Approved},
                new Power{PowerName="Laser Vision",PowerDescription="Being able to omit power from eyes",
                    OwnerID = adminID,PowerType=PowerType.Superhuman,Status=PowerStatus.Approved},
                new Power{PowerName="Archery",PowerDescription="Art, sport, practice, or skill of using a bow to shoot arrows.",
                    OwnerID = adminID,PowerType=PowerType.Human,Status=PowerStatus.Approved}
            };
            foreach (Power p in powers)
            {
                context.Powers.Add(p);
            }
            context.SaveChanges();

            var characterPowerEntries = new CharacterPowerEntry[]
            {
                new CharacterPowerEntry{CharacterID = characters.Single(s => s.Alias == "Ironman").CharacterID,
                    PowerID = powers.Single(c => c.PowerName == "Super Strength" ).PowerID,
                    Specification="When he wears the Ironman-suit.",
                    Level=Level.Strong},
                new CharacterPowerEntry{CharacterID =  characters.Single(s => s.Alias == "Ironman").CharacterID,
                    PowerID = powers.Single(c => c.PowerName == "Intelligence" ).PowerID,
                    Specification="Natural-born genius",
                    Level=Level.Strong},
                new CharacterPowerEntry{CharacterID =  characters.Single(s => s.Alias == "Ironman").CharacterID,
                    PowerID = powers.Single(c => c.PowerName == "Flying" ).PowerID,
                    Specification="When he wears the Ironman-suit.",
                    Level=Level.Medium},
                new CharacterPowerEntry{CharacterID = characters.Single(s => s.Alias == "Hulk").CharacterID,
                    PowerID = powers.Single(c => c.PowerName == "Super Strength" ).PowerID,
                    Specification="When Dr Banner transforms to Hulk.",
                    Level=Level.Strong},
                new CharacterPowerEntry{CharacterID = characters.Single(s => s.Alias == "Hulk").CharacterID,
                    PowerID = powers.Single(c => c.PowerName == "Intelligence" ).PowerID,
                    Specification="Dr Banner is natural-born genius, but as Hulk, he loses this intellect in echange of super-strength.",
                    Level=Level.Strong},
                new CharacterPowerEntry{CharacterID = characters.Single(s => s.Alias == "Superman").CharacterID,
                    PowerID = powers.Single(c => c.PowerName == "Super Strength" ).PowerID,
                    Specification="As a son of Krypton, his strength is naturally superhuman.",
                    Level=Level.Strong},
                new CharacterPowerEntry{CharacterID = characters.Single(s => s.Alias == "Superman").CharacterID,
                    PowerID = powers.Single(c => c.PowerName == "Flying" ).PowerID,
                    Specification="As a son of Krypton, being able to fly is a natural ability.",
                    Level=Level.Strong}
            };
            foreach (CharacterPowerEntry cp in characterPowerEntries)
            {
                context.CharacterPowerEntries.Add(cp);
            }
            context.SaveChanges();

            var teams = new Team[]
            {
                new Team{TeamName="Avengers",TeamMotto="Assembly",Side=Side.Good,OwnerID = adminID,Status=TeamStatus.Approved, ProfileImage = new byte[]{ },
                    Introduction="Team of the 'Earth's Mightiest Heroes' featuring  humans, superhumans, mutants, inhumans, deities, androids, aliens, legendary beings, and even former villains."},
                new Team{TeamName="Justice League",TeamMotto="Unite the league", Side=Side.Good,OwnerID = adminID,Status=TeamStatus.Approved, ProfileImage = new byte[]{ },
                    Introduction="Team of the superheroes formed to protect Earth firstly from alien invasion, then from all kind of dangers."},
                new Team{TeamName="Hydra",TeamMotto="Heil Hydra", Side=Side.Bad,OwnerID = adminID,Status=TeamStatus.Approved, ProfileImage = new byte[]{ },
                    Introduction="Authoritarian paramilitary-subversive organization founded in ancient times, what bent on world domination."}
            };
            foreach (Team t in teams)
            {
                context.Teams.Add(t);
            }
            context.SaveChanges();

            var teamentries = new TeamEntry[]
            {
                new TeamEntry{CharacterID = characters.Single(s => s.Alias == "Ironman").CharacterID,
                    TeamID = teams.Single(c => c.TeamName == "Avengers" ).TeamID,
                    Role=Role.Leader},
                new TeamEntry{CharacterID = characters.Single(s => s.Alias == "Hulk").CharacterID,
                    TeamID = teams.Single(c => c.TeamName == "Avengers" ).TeamID,
                    Role=Role.Member},
                new TeamEntry{CharacterID = characters.Single(s => s.Alias == "Superman").CharacterID,
                    TeamID = teams.Single(c => c.TeamName == "Justice League" ).TeamID,
                    Role=Role.Member},
                new TeamEntry{CharacterID = characters.Single(s => s.Alias == "Red Skull").CharacterID,
                    TeamID = teams.Single(c => c.TeamName == "Hydra" ).TeamID,
                    Role=Role.Leader}
            };
            foreach (TeamEntry t in teamentries)
            {
                context.TeamEntries.Add(t);
            }
            context.SaveChanges();

            var universeentries = new UniverseEntry[]
            {
                new UniverseEntry{UniverseID = universes.Single(s => s.Name == "MCU").UniverseID,
                    TeamID = teams.Single(c => c.TeamName == "Avengers" ).TeamID, NewMember=NewMember.Team},
                new UniverseEntry{UniverseID = universes.Single(s => s.Name == "MCU").UniverseID,
                    CharacterID = characters.Single(c => c.Alias == "Mad Titan" ).CharacterID, NewMember=NewMember.Character},
                new UniverseEntry{UniverseID = universes.Single(s => s.Name == "DCU").UniverseID,
                    TeamID = teams.Single(c => c.TeamName == "Justice League" ).TeamID, NewMember=NewMember.Team},
                new UniverseEntry{UniverseID = universes.Single(s => s.Name == "MCU").UniverseID,
                    TeamID = teams.Single(c => c.TeamName == "Hydra" ).TeamID, NewMember=NewMember.Team},
                new UniverseEntry{UniverseID = universes.Single(s => s.Name == "DCU").UniverseID,
                    CharacterID = characters.Single(c => c.Alias == "The Joker" ).CharacterID, NewMember=NewMember.Character}
            };
            foreach (UniverseEntry u in universeentries)
            {
                context.UniverseEntries.Add(u);
            }
            context.SaveChanges();

            var opponententries = new OpponentEntry[]
            {
                 new OpponentEntry{CharacterID=characters.Single(s => s.Alias == "Superman").CharacterID,
                                   OpponentCharacterID=characters.Single(s => s.Alias == "Lex Luthor").CharacterID,
                                   OpponentsUniverse = context.Universes.Single(x=>x.Name == "DCU")},
                 new OpponentEntry{TeamID=teams.Single(s => s.TeamName == "Justice League").TeamID,
                                   OpponentCharacterID=characters.Single(s => s.Alias == "Lex Luthor").CharacterID,
                                   OpponentsUniverse = context.Universes.Single(x=>x.Name == "DCU")},
                 new OpponentEntry{TeamID=teams.Single(s => s.TeamName == "Avengers").TeamID,
                                   OpponentCharacterID = characters.Single(s => s.Alias == "Mad Titan").CharacterID,
                                   OpponentsUniverse = context.Universes.Single(x=>x.Name == "MCU")},
                 new OpponentEntry{TeamID=teams.Single(s => s.TeamName == "Avengers").TeamID,
                                   OpponentTeamID = teams.Single(s => s.TeamName == "Hydra").TeamID, 
                                   OpponentsUniverse = context.Universes.Single(x=>x.Name == "MCU")},
            };
            foreach (OpponentEntry o in opponententries)
            {
                context.OpponentEntries.Add(o);
            }
            context.SaveChanges();
        }

    }
}
