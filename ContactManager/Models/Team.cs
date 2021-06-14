using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum Side
    {
        Good, Grey, Bad
    }

    public enum TeamStatus
    {
        Submitted, Approved, Rejected
    }

    public class Team
    {
        [Key]
        public int TeamID { get; set; }

        public string OwnerID { get; set; }

        public byte[] ProfileImage { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Team Name")]
        public string TeamName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Team Motto")]
        [DisplayFormat(NullDisplayText = "No team motto")]
        public string TeamMotto { get; set; }

        [StringLength(500, MinimumLength = 2)]
        [DisplayFormat(NullDisplayText = "No introduction")]
        public string Introduction { get; set; }

        public TeamStatus Status { get; set; }

        [Required]
        public Side Side { get; set; }

        [Display(Name = "Universe(s)")]
        public ICollection<UniverseEntry> UniverseEntries { get; set; }

        [Display(Name = "Team Members")]
        public ICollection<TeamEntry> TeamEntries { get; set; }

        [Display(Name = "Opponent(s)")]
        public ICollection<OpponentEntry> OpponentEntries { get; set; }
    }
}
