using System.Net;
using AlutaMartAPI.Services;
using Microsoft.AspNetCore.Diagnostics;
using Sentry;

namespace AlutaMartAPI.Utilities;

public static class ExceptionMiddlewareExtensions
{
	public static void UseExceptionHandler(this IApplicationBuilder app, ILoggerFactory logger)
	{
		var _logger = logger.CreateLogger("ConfigureExceptionHandler");
		_ = app.UseExceptionHandler(appError =>
		{
			appError.Run(async context =>
			{
				context.Response.StatusCode = (int)HttpStatusCode.OK;
				context.Response.ContentType = "application/json";

				var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
				if (contextFeature != null)
				{
					_logger.LogError("An error occured__: {error}", contextFeature.Error);
					SentrySdk.CaptureException(contextFeature.Error);
					await context.Response.WriteAsync(new ResponseService().ErrorResponse<string>("An error occurred!").ToJson());
				}
			});
		});
	}
}
