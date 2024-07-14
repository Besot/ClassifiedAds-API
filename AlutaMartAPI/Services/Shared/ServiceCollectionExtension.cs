

using AlutaMartAPI.Database;

namespace AlutaMartAPI.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAlutaMartServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IVendorService, VendorService>();


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