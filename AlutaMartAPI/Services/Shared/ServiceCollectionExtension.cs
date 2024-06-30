

using AlutaMartAPI.Database;

namespace AlutaMartAPI.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAlutaMartServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IResponseService, ResponseService>();
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