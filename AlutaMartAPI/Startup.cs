using System.Reflection;
using Microsoft.OpenApi.Models;
using AlutaMartAPI.Database;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI;

public class Startup
{
	public Startup(IWebHostEnvironment env)
	{
		var builder = new ConfigurationBuilder()
		  .SetBasePath(env.ContentRootPath)
		  .AddEnvironmentVariables();
		Configuration = builder.Build();
	}

	public IConfiguration Configuration { get; }
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddDataServices();
		services.AddIdentityServices();
		services.AddAlutaMartServices();
		services.AddHttpClient();
		services.AddCors(o => o.AddPolicy("CoresPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
		services.AddControllers();
		services.ResponseCompression();
		services.AddAuthenticationServices();

		services.AddLogging(options => options.AddSimpleConsole(c => c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] "));

		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Version = "v1",
				Title = "AlutaMart API",
				TermsOfService = new Uri("https://api.alutamart.co"),
				License = new OpenApiLicense
				{
					Name = "AlutaMart API",
					Url = new Uri("https://api.alutamart.co")
				},
			});
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Enter JWT with Bearer token into this field",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement()
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						},
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header,
					},
					new List<string>()
				}
			});
			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    		var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    		c.IncludeXmlComments(xmlPath);
		});
	}

	public static void Configure(IApplicationBuilder app, ILoggerFactory logger)
	{
		app.UseExceptionHandler(logger);
		app.UseRouting();
		app.UseCors("CoresPolicy");
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseStaticFiles();
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.RoutePrefix = "swagger";
			c.DefaultModelsExpandDepth(-1);
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
		});

		app.Use(async (context, next) =>
		{
			context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
			context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
			context.Response.Headers.Append("X-Download-Options", "noopen");
			context.Response.Headers.Append("X-Frame-Options", "DENY");
			context.Response.Headers.Append("Content-Security-Policy",
				"object-src 'none'," +
				"media-src 'none'," +
				"form-action 'self',");
			await next(context);
		});

		app.UseEndpoints(endpoints => endpoints.MapControllers());
		app.InitializeDatabase();
		app.SeedDataToDatabase();
	}
}
