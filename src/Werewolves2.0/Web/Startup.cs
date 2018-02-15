using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Web.Bot;

namespace Web
{
    public class Startup
    {
		private DiscordBot bot;

		public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			IConfigurationRoot configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json")
			.Build();

			services.AddDbContext<DIdentityDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString("Accounts"),
			optionsBuilder => optionsBuilder.MigrationsAssembly("Web"))); //Also at DeisgnTimeDDbContextFactory
			services.AddIdentity<DIdentityUser, DIdentityRole>(o =>
			{
				//Password
				o.Password.RequireDigit = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequireLowercase = false;
				o.Password.RequiredUniqueChars = 2;
				o.Password.RequiredLength = 6;
				//SignIn
				o.SignIn.RequireConfirmedEmail = false;
				o.SignIn.RequireConfirmedPhoneNumber = false;
			})
				.AddEntityFrameworkStores<DIdentityDbContext>()
				.AddDefaultTokenProviders();

			services.AddAuthorization(options =>
			{
				options.AddPolicy("Authenticated",
					policy => policy.RequireAuthenticatedUser());
			});

			services.AddMvc();

			SetupDiscordBot();
			Task.Run(() => bot.RunAsync());
		}

		private void SetupDiscordBot()
		{
			this.bot = new DiscordBot();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
			app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
