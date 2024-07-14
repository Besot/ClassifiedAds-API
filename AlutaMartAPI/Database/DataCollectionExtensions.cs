using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;
namespace AlutaMartAPI.Database
{
    public static class DataCollectionExtensions
    {
        public static void AddDataServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(
                opt => 
                {
                    opt.UseNpgsql(Constants.DBConnection);
                    opt.ConfigureWarnings(builder => builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
                }
            );
        }

        public static void AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<Profile, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddAuthenticationServices(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Constants.JWTIssuerAndAudience,
                    ValidAudience = Constants.JWTIssuerAndAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JWTKey))
                };
            });
        }

        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count != 0)
            {
                context.Database.Migrate();
            }
        }

        public static async Task SeedDataToDatabase(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Profile>>();


            if(!context.Currencies.Any()) context.AddRange(DataSeed.GetCurrencies());   

            if(!context.Institutions.Any()) context.AddRange(DataSeed.GetInstitutions());

            if(!context.AdsCategories.Any()) context.AddRange(DataSeed.GetAdsCategories());

            if(!context.PlanTiers.Any()) context.AddRange(DataSeed.GetPlanTiers());
            
            if (!context.Profiles.Any())
                {
                    var superAdmins = await DataSeed.GetSuperAdmin(userManager);
                    context.AddRange(superAdmins);
                }

                        context.SaveChanges();
                        context.Dispose();
        }
    }
}