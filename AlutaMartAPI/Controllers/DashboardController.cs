using AlutaMartAPI.Services;
using Microsoft.AspNetCore.Mvc;
using AlutaMartAPI.Utilities;
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Controllers;

namespace PatherAPI.Controllers;

public class DashboardController(IDashboardService dashboardService) : BaseController
{
	[HttpGet("Analytics"), AccessControl([Roles.PlatformManager, Roles.SuperAdmin], AccessType.Allow)]
	[ProducesResponseType(type: typeof(ServiceResponse<AdminDashboardDTO>), statusCode: 200)]
	public async Task<IActionResult> GetAnalytics() => Ok(await dashboardService.GetAnalyticsAsync());
}
