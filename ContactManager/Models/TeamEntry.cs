using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum Role
    {
        Leader, Member
    }

    public class TeamEntry
    {
        public int TeamID { get; set; }
        public int CharacterID { get; set; }

        public Role? Role { get; set; }

        public Team Team { get; set; }
        public Character Character { get; set; }
    }
}
