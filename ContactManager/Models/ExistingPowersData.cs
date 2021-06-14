using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public class ExistingPowersData
    {
        public int PowerID { get; set; }
        public string PowerName { get; set; }

        #nullable enable
        [StringLength(100, MinimumLength = 2)]
        public string? PowerDescription { get; set; }
        public SelectList? Levels { get; set; }
        public Level? Level { get; set; }
        #nullable disable

        public bool Existing { get; set; }
    }
}
