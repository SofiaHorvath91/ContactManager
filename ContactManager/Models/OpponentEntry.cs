using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum OpponentStatus
    {
        Submitted, Approved, Rejected
    }

    public class OpponentEntry
    {
        [Key]
        public int OpponentEntryID { get; set; }

        public string OwnerID { get; set; }

        #nullable enable
        public int? CharacterID { get; set; }
        public int? TeamID { get; set; }
        public int? OpponentCharacterID { get; set; }
        public int? OpponentTeamID { get; set; }
        public string? Item { get; set; }
        public string? OpponentItem { get; set; }
#nullable disable

        public OpponentStatus Status { get; set; }

        public Universe OpponentsUniverse { get; set; }

        public Character Character { get; set; }
        public Team Team { get; set; }
    }
}
