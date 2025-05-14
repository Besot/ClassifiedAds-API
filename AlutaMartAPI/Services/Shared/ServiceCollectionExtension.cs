using AlutaMartAPI.Database;
using AlutaMartAPI.Services.Classes;
using AlutaMartAPI.Utilities;
using AspNetCoreRateLimit;

namespace AlutaMartAPI.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAlutaMartServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();
            
            services.AddScoped<IAdsService, AdsService>();
            services.AddScoped<IPlanTierService, PlanTierService>();

            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IInstitutionService, InstitutionService>();


            services.AddTransient<IBaseHttpClient, BaseHttpClient>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IMailSenderService, MailSenderService>();
            services.AddTransient<IPaystackService, PaystackService>();
            services.AddScoped<IWalletService, WalletServices>();
            services.AddSingleton<ExpiredAdCheckService>();
            
            // New services
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IRatingService, RatingService>();

            services.AddHostedService<ExpiredAdCheckService>();

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddInMemoryRateLimiting();

            services.AddHttpClient<IGeocodingService, GeocodingService>(client =>
            {
                // Optionally set base address or other HttpClient settings
            });

            // Register the Google API key from configuration
            services.AddSingleton<IGeocodingService>(provider => new GeocodingService(
                provider.GetRequiredService<HttpClient>(),
                Constants.ApiKey)); // Replace with your API key


            services.AddMemoryCache();

            services.AddResponseCompression(options =>
            {
                options.MimeTypes =
                [
                    "text/plain",
                    "text/css",
                    "application/javascript",
                    "text/html",
                    "application/json",
                    "text/json"
                ];
                options.EnableForHttps = true;
            });

            services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules =
                [
                    new() {
                        // Update to match your route
                        Endpoint = "*:/v1/auth/login", // Limit for the login endpoint
                        Limit = 5, // Max 5 requests
                        Period = "5m" // Per 1 minute
                    }
                ];
            });            
        }

        public static void ResponseCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.MimeTypes =
                [
                    "text/plain",
                    "text/css",
                    "application/javascript",
                    "text/html",
                    "application/json",
                    "text/json"
                ];
                options.EnableForHttps = true;
            });
        }
    }
}