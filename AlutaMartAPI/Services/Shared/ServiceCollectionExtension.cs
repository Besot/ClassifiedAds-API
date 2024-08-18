

using AlutaMartAPI.Database;

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


            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<IAdsCategoryService, AdsCategoryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IInstitutionService, InstitutionService>();


            services.AddTransient<IBaseHttpClient, BaseHttpClient>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IMailSenderService, MailSenderService>();

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