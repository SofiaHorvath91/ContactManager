using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum Level
    {
        Weak, Medium, Strong
    }

    public class CharacterPowerEntry
    {
        public int CharacterID { get; set; }
        public int PowerID { get; set; }
        public string Specification { get; set; }

        public Level? Level { get; set; }

        public Character Character { get; set; }
        public Power Power { get; set; }
    }
}
