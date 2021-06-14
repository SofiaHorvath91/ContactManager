using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum FavType
    {
        Character, Team, Universe
    }

    public class Favourite
    {
        [Key]
        public int FavouriteID { get; set; }

        public string OwnerID { get; set; }

        public int SelectedFavID { get; set; }

        public FavType FavType { get; set; }
    }
}
