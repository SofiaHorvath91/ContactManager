using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace ContactManager.Pages.Universes
{
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(context, authorizationService, userManager, configuration, environment)
        {
        }

        public string NameSort { get; set; }
        public string OriginSort { get; set; }
        public string GenreSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Universe> Universe { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;

            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            OriginSort = sortOrder == "Origin" ? "origin_desc" : "Origin";
            GenreSort = sortOrder == "Genre" ? "genre_desc" : "Genre";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            var universes = from u in Context.Universes
                           select u;

            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized)
            {
                universes = universes.Where(c => c.Status == UniverseStatus.Approved
                                            || c.OwnerID == currentUserId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                universes = universes.Where(s => s.Name.Contains(searchString)
                                       || s.Creator.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    universes = universes.OrderByDescending(s => s.Name);
                    break;
                case "Origin":
                    universes = universes.OrderBy(s => s.Origin);
                    break;
                case "origin_desc":
                    universes = universes.OrderByDescending(s => s.Origin);
                    break;
                case "Genre":
                    universes = universes.OrderBy(s => s.Genre);
                    break;
                case "genre_desc":
                    universes = universes.OrderByDescending(s => s.Genre);
                    break;
                default:
                    universes = universes.OrderBy(s => s.Name);
                    break;
            }

            var pageSize = Configuration.GetValue("PageSize", 10);
            Universe = await PaginatedList<Universe>.CreateAsync(
                universes.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
