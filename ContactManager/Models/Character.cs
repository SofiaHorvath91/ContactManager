using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum Moral
    {
        Good, Grey, Bad
    }

    public enum Kind
    {
        Human, Superhuman, Alien, Mutant
    }

    public enum CharacterStatus
    {
        Submitted, Approved, Rejected
    }

    public class Character
    {
        [Key]
        public int CharacterID { get; set; }

        public string OwnerID { get; set; }

        public byte[] ProfileImage { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Real Name")]
        public string RealName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Alias { get; set; }

        [StringLength(500, MinimumLength = 2)]
        [DisplayFormat(NullDisplayText = "No introduction")]
        public string Introduction { get; set; }

        public CharacterStatus Status { get; set; }

        [Required]
        public Moral Moral { get; set; }

        [Required]
        public Kind Kind { get; set; }

        [Display(Name = "Universe(s)")]
        public ICollection<UniverseEntry> UniverseEntries { get; set; }

        [Display(Name = "Powers")]
        public ICollection<CharacterPowerEntry> CharacterPowerEntries { get; set; }

        [Display(Name = "Team(s)")]
        public ICollection<TeamEntry> TeamEntries { get; set; }

        [Display(Name = "Opponent(s)")]
        public ICollection<OpponentEntry> OpponentEntries { get; set; }

    }
}
