using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public class ExistingUniversesData
    {
        public int UniverseID { get; set; }
        public string UniverseName { get; set; }

        #nullable enable
        public List<string>? Teams { get; set; }
        #nullable disable

        public bool Existing { get; set; }
    }
}
