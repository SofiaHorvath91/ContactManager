using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum PowerType
    {
        Human, Superhuman, Both
    }

    public enum PowerStatus
    {
        Submitted, Approved, Rejected
    }

    public class Power
    {
        [Key]
        public int PowerID { get; set; }

        public string OwnerID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Power Name")]
        public string PowerName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Power Description")]
        public string PowerDescription { get; set; }

        public PowerStatus Status { get; set; }

        [Display(Name = "Power Type")]
        [DisplayFormat(NullDisplayText = "No power type specified")]
        public PowerType? PowerType { get; set; }

        [Display(Name = "Characters with such power")]
        public ICollection<CharacterPowerEntry> CharacterPowerEntries { get; set; }
    }
}
