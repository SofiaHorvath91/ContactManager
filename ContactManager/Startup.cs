using ContactManager.Authorization;
using ContactManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddRazorPages();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "--PUT YOUR OWN DATA--";
                    options.ClientSecret = "--PUT YOUR OWN DATA--";
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = "--PUT YOUR OWN DATA--";
                    facebookOptions.AppSecret = "--PUT YOUR OWN DATA--";
                    facebookOptions.AccessDeniedPath = "/AccessDeniedPathInfo";
                })
                .AddTwitter(twitterOptions =>
                {
                    twitterOptions.ConsumerKey = "--PUT YOUR OWN DATA--";
                    twitterOptions.ConsumerSecret = "--PUT YOUR OWN DATA--";
                    twitterOptions.RetrieveUserDetails = true;
                });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddScoped<IAuthorizationHandler, CharacterIsOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, PowerIsOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, TeamIsOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, UniverseIsOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, FavouriteIsOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, WordIsOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, OpponentIsOwnerAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler, CharacterAdministratorsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, PowerAdministratorsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, TeamAdministratorsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, UniverseAdministratorsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, FavouriteAdministratorsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, WordAdministratorsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, OpponentAdministratorsAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseForwardedHeaders();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
