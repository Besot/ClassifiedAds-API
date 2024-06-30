using AlutaMartAPI.Utilities;

namespace AlutaMartAPI;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    { 
        DotNetEnv.Env.Load();
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(p => 
                {
                    p.AddServerHeader = false;
                    p.Limits.MinRequestBodyDataRate = null;
                    p.Limits.MaxConcurrentConnections = null;
                    p.Limits.MaxRequestBodySize = null;
                    p.AllowSynchronousIO = true;
                });
                webBuilder.UseUrls("http://*:1993");
                // webBuilder.UseSentry(o =>
                // {
                //     o.Dsn = Constants.SentryKey;
                //     o.Debug = false;
                //     o.TracesSampleRate = 1.0;
                // });
                webBuilder.UseStartup<Startup>();
            });
    }
}