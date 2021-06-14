using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ContactManager.Models;

namespace ContactManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<Power> Powers { get; set; }
        public DbSet<CharacterPowerEntry> CharacterPowerEntries { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamEntry> TeamEntries { get; set; }
        public DbSet<OpponentEntry> OpponentEntries { get; set; }
        public DbSet<Universe> Universes { get; set; }
        public DbSet<UniverseEntry> UniverseEntries { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Word> Words { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Word>().ToTable("Word");
            modelBuilder.Entity<Favourite>().ToTable("Favourite");
            modelBuilder.Entity<Character>().ToTable("Character");
            modelBuilder.Entity<Power>().ToTable("Power");
            modelBuilder.Entity<CharacterPowerEntry>().ToTable("CharacterPowerEntry");
            modelBuilder.Entity<Team>().ToTable("Team");
            modelBuilder.Entity<TeamEntry>().ToTable("TeamEntry");
            modelBuilder.Entity<OpponentEntry>().ToTable("OpponentEntry");
            modelBuilder.Entity<Universe>().ToTable("Universe");
            modelBuilder.Entity<UniverseEntry>().ToTable("UniverseEntry");

            modelBuilder.Entity<CharacterPowerEntry>()
                .HasKey(c => new { c.CharacterID, c.PowerID });

            modelBuilder.Entity<TeamEntry>()
                .HasKey(c => new { c.TeamID, c.CharacterID });
        }
    }
}
