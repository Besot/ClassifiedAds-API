using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;
using System.Reflection;
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

        public static void SeedDataToDatabase(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var dataSeed = new DataSeed(webHostEnvironment);

            // Fetch and update AdsCategories
            var adsCategories = dataSeed.GetAdsCategories().ToList();
            context.UpdateDatabase(
                adsCategories,
                record => record.Code, // 'Code' is the unique key for AdsCategory
                (existing, updated) => {
                    existing.Name = updated.Name;
                    existing.Brand = updated.Brand;
                    // Add or update other properties as needed
                });

            // Fetch and update Currencies
            var currencies = dataSeed.GetCurrencies().ToList();
            context.UpdateDatabase(
                currencies,
                record => record.Code, // 'Code' is the unique key for Currency
                (existing, updated) => {
                    existing.Name = updated.Name;
                    // Add or update other properties as needed
                });

            // Fetch and update Institutions
            var institutions = dataSeed.GetInstitutions().ToList();
            context.UpdateDatabase(
                institutions,
                record => record.Name, // 'Name' is the unique key for Institution
                (existing, updated) => {
                    existing.Abbrev = updated.Abbrev;
                    existing.State = updated.State;
                    // Add or update other properties as needed
                });

            // Fetch and update PlanTiers
            var planTiers = dataSeed.GetPlanTiers().ToList();
            context.UpdateDatabase(
                planTiers,
                record => record.Name, // 'Name' is the unique key for PlanTier
                (existing, updated) => {
                    existing.Amount = updated.Amount;
                    existing.MaxAds = updated.MaxAds;
                    existing.MaxPicture = updated.MaxPicture;
                    existing.MaxFeatured = updated.MaxFeatured;
                    // Update other properties as needed
                });

            context.SaveChanges();
        }



        public static void UpdateDatabase<T>(
            this AppDbContext context,
            IEnumerable<T> csvRecords,
            Func<T, object> keySelector,
            Action<T, T> updateAction) where T : class
        {
            var existingRecords = context.Set<T>()
                .AsNoTracking()
                .ToList();

            // Create a dictionary of existing records with their keys
            var existingRecordDict = existingRecords.ToDictionary(keySelector);

            var newRecords = new List<T>();
            var updatedRecords = new List<T>();

            foreach (var csvRecord in csvRecords)
            {
                var key = keySelector(csvRecord);
                if (existingRecordDict.TryGetValue(key, out var existingRecord))
                {
                    // Record exists, check if it needs an update
                    if (RecordNeedsUpdate(existingRecord, csvRecord))
                    {
                        updateAction(existingRecord, csvRecord);
                        updatedRecords.Add(existingRecord);
                    }
                    existingRecordDict.Remove(key); // Remove from dictionary once processed
                }
                else
                {
                    // Record does not exist, add it as a new record
                    newRecords.Add(csvRecord);
                }
            }

            using var transaction = context.Database.BeginTransaction();
            try
            {
                if (newRecords.Count != 0)
                {
                    context.Set<T>().AddRange(newRecords);
                }

                if (updatedRecords.Count != 0)
                {
                    context.Set<T>().UpdateRange(updatedRecords);
                }

                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                // Log and handle exception
                throw new InvalidOperationException("An error occurred while updating the database.", ex);
            }
        }

        private static bool RecordNeedsUpdate<T>(T existingRecord, T csvRecord) where T : class
        {
            if (existingRecord == null || csvRecord == null)
            {
                throw new ArgumentNullException("Records to compare cannot be null.");
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var existingValue = property.GetValue(existingRecord);
                var csvValue = property.GetValue(csvRecord);

                // Check if the property values are different
                if (existingValue == null && csvValue != null ||
                    existingValue != null && !existingValue.Equals(csvValue))
                {
                    return true; // Record needs update
                }
            }

            return false; // No updates needed
        }
    }
}