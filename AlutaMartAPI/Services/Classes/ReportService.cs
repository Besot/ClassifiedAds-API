using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services
{
    public class ReportService(IUnitOfWork unitOfWork, IResponseService responseService) : IReportService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IResponseService _responseService = responseService;

        public async Task<ServiceResponse<string>> CreateReportAsync(CreateReportDTO model, UserDTO currentUser)
        {
            // 1) Description required
            if (string.IsNullOrWhiteSpace(model.Description))
                return _responseService.ErrorResponse<string>("Please provide a description of the issue");

            // 2) Must report either an Ad or a Vendor
            if (model.AdsId == null && model.VendorId == null)
                return _responseService.ErrorResponse<string>("Please specify either an ad or a vendor to report");

            // 3) If AdsId provided, ensure it exists
            if (model.AdsId.HasValue)
            {
                bool adExists = await _unitOfWork.Context.Ads
                    .AsNoTracking()
                    .AnyAsync(a => a.Id == model.AdsId.Value);
                if (!adExists)
                    return _responseService.ErrorResponse<string>("Ad not found");
            }

            // 4) If VendorId provided, ensure it exists
            if (model.VendorId.HasValue)
            {
                bool vendorExists = await _unitOfWork.Context.Reports
                    .AsNoTracking()
                    .AnyAsync(v => v.Id == model.VendorId.Value);
                if (!vendorExists)
                    return _responseService.ErrorResponse<string>("Vendor not found");
            }

            // 5) Persist the report
            var report = new Report
            {
                Description = model.Description,
                Type        = model.Type,
                AdsId       = model.AdsId,
                VendorId    = model.VendorId,
                ReporterProfileId  = currentUser.Id,
            };

            await _unitOfWork.Context.AddAsync(report);
            await _unitOfWork.CommitAsync();

            return _responseService.SuccessResponse(report.Id.ToString());
        }

        public async Task<ServiceResponse<GetReportDTO>> GetReportDetailsAsync(Guid reportId)
        {
            var report = await _unitOfWork.Context.Reports
                .AsNoTracking()
                .Include(b => b.ReporterProfile)
                .Include(r => r.Ads)
                    .ThenInclude(ad => ad.Vendor)
                .Include(r => r.Ads)
                    .ThenInclude(ad => ad.AdsCategories)
                        .ThenInclude(ac => ac.Category)
                .Include(r => r.Vendor)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                return _responseService.ErrorResponse<GetReportDTO>("Report not found");

            var dto = new GetReportDTO
            {
                Id           = report.Id,
                Description  = report.Description,
                Type         = report.Type.ToString(),
                Status       = report.Status.ToString(),
                AdminNote    = report.AdminNote,
                CreatedAt    = report.Created,
                ReporterName = $"{report?.ReporterProfile?.FirstName} {report.ReporterProfile?.LastName}"
            };

            if (report.Ads != null)
            {
                dto.ReportedAd = new GetAdsDTO
                {
                    Id          = report.Ads.Id,
                    Title       = report.Ads.Title,
                    Description = report.Ads.Description,
                    Price       = report.Ads.Price,
                    Status      = report.Ads.Status.Name(),
                    VendorId    = report.Ads.VendorId,
                    BrandName  = report.Ads.Vendor?.BrandName
                };
            }

            if (report.Vendor != null)
            {
                dto.ReportedVendor = new GetVendorDTO
                {
                    Id        = report.Vendor.Id,
                    BrandName = report.Vendor.BrandName,
                    Bio       = report.Vendor.Bio,
                    Rating    = report.Vendor.Rating
                };
            }

            return _responseService.SuccessResponse(dto);
        }

        public async Task<ServiceResponse<PagedList<GetReportDTO>>> GetReportsAsync(int page = 1, int pageSize = 20)
        {
            var query =  _unitOfWork.Context.Reports
                .AsNoTracking()
                .Include(r => r.ReporterProfile)
                .Include(r => r.Ads)
                .Include(r => r.Vendor)
                .OrderByDescending(r => r.Created);

            return await _responseService.PagedResponseAsync(
                query.Select(r => new GetReportDTO
                {
                    Id           = r.Id,
                    Description  = r.Description,
                    Type         = r.Type.ToString(),
                    Status       = r.Status.ToString(),
                    AdminNote    = r.AdminNote,
                    CreatedAt    = r.Created,
                    ReporterName = $"{r.ReporterProfile.FirstName} {r.ReporterProfile.LastName}",
                    ReportedAd = r.Ads != null ? new GetAdsDTO
                    {
                        Id    = r.Ads.Id,
                        Title = r.Ads.Title
                    } : null,
                    ReportedVendor = r.Vendor != null ? new GetVendorDTO
                    {
                        Id        = r.Vendor.Id,
                        BrandName = r.Vendor.BrandName
                    } : null
                }),
                page,
                pageSize,
                "Reports"
            );
        }

        public async Task<ServiceResponse<string>> UpdateReportStatusAsync(
            Guid reportId,
            UpdateReportStatusDTO model,
            UserDTO currentUser)
        {
            if (currentUser.Access != Roles.AdminUser 
                && currentUser.Access != Roles.SuperAdmin)
            {
                return _responseService.ErrorResponse<string>(
                    "Only administrators can update report status");
            }

            var report = await _unitOfWork.Context.Reports
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                return _responseService.ErrorResponse<string>("Report not found");

            report.Status    = model.Status;
            report.AdminNote = model.AdminNote;
            report.Modified = DateTimeOffset.UtcNow;

            await _unitOfWork.CommitAsync();
            return _responseService.SuccessResponse("Report status updated successfully");
        }
    }
}
