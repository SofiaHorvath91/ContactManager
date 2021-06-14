using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum NewMember
    {
        Team, Character
    }

    public class UniverseEntry
    {
        [Key]
        public int UniverseEntryID { get; set; }

        public int UniverseID { get; set; }
        public int? TeamID { get; set; }
        public int? CharacterID { get; set; }

        public NewMember NewMember { get; set; }

        public Universe Universe { get; set; }
        public Character Character { get; set; }
        public Team Team { get; set; }
    }
}
