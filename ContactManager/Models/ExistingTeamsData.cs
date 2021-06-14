using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public class ExistingTeamsData
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }

        #nullable enable
        public SelectList? Roles { get; set; }
        public Role? Role { get; set; }
        #nullable disable

        public bool Existing { get; set; }
    }
}
