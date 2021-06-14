using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public enum Origin
    {
        Book, Comic, Film, Series, Private
    }

    public enum Genre
    {
        SciFi, Fantasy, Supernatural, Superhero, Crime, Anime, Manga
    }

    public enum UniverseStatus
    {
        Submitted, Approved, Rejected
    }

    public class Universe
    {
        [Key]
        public int UniverseID { get; set; }

        public string OwnerID { get; set; }

        public byte[] ProfileImage { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Universe Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Universe Creator")]
        public string Creator { get; set; }

        public UniverseStatus Status { get; set; }

        [Required]
        public Origin Origin { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Display(Name = "Teams & Characters")]
        public ICollection<UniverseEntry> UniverseEntries { get; set; }
    }
}
