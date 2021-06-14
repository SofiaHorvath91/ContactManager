using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum WordStatus
    {
        Submitted, Approved, Rejected
    }

    public class Word
    {
        [Key]
        public int WordID { get; set; }

        public string OwnerID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Word")]
        public string WordString { get; set; }

        public WordStatus Status { get; set; }

        public Universe Fandom { get; set; }
    }
}
