using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AlutaMartAPI.Controllers
{
    public class ReportController(IReportService reportService) : BaseController
    {
        [HttpPost, AllowAccess(Roles.Buyer)]
        [ProducesResponseType(typeof(ServiceResponse<string>), 200)]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDTO model)
            => Ok(await reportService.CreateReportAsync(model, CurrentUser));

        [HttpGet, AccessControl([Roles.AdminUser, Roles.SuperAdmin], AccessType.Allow)]
        [ProducesResponseType(typeof(ServiceResponse<PagedList<GetReportDTO>>), 200)]
        public async Task<IActionResult> GetReports(int page = 1, int pageSize = 20)
            => Ok(await reportService.GetReportsAsync(page, pageSize));

        [HttpGet("{reportId}"),  AccessControl([Roles.AdminUser, Roles.SuperAdmin], AccessType.Allow)]
        [ProducesResponseType(typeof(ServiceResponse<GetReportDTO>), 200)]
        public async Task<IActionResult> GetReportDetails(Guid reportId)
            => Ok(await reportService.GetReportDetailsAsync(reportId));

        [HttpPut("{reportId}"),  AccessControl([Roles.AdminUser, Roles.SuperAdmin], AccessType.Allow)]
        [ProducesResponseType(typeof(ServiceResponse<string>), 200)]
        public async Task<IActionResult> UpdateReportStatus(Guid reportId, [FromBody] UpdateReportStatusDTO model)
            => Ok(await reportService.UpdateReportStatusAsync(reportId, model, CurrentUser));
    }
}
