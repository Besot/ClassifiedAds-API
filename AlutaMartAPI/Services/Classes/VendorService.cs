using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.SQLQueries;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AlutaMartAPI.Services;
public class VendorService(IUnitOfWork _unitOfWork, IResponseService _responseService, INotificationService _notificationService) : IVendorService
{
    public async Task<ServiceResponse<string>> CreateAsync(CreateVendorDTO model, UserDTO user)
    {
        if(model.VendorInstitutionId == null || !model.VendorInstitutionId.HasValue) return _responseService.ErrorResponse<string>("Vendor's institution is required");

        var isAlreadyVendor = await _unitOfWork.Context.Vendors.AnyAsync(x => x.ProfileId == user.Id);
        if(isAlreadyVendor) 
        {
            _notificationService.VendorOnboardingEmailAsync(user.Email, user.FirstName).Forget();
            return _responseService.SuccessResponse("User is already a vendor");
        }

        var validInstitution = await _unitOfWork.Context.VendorInstitutions.AnyAsync(x => x.Id == model.VendorInstitutionId.Value);
        if(!validInstitution) return _responseService.ErrorResponse<string>("Invalid request");

        var vendor = new Vendor
        {
            ProfileId = user.Id,
            VendorInstitutionId = model.VendorInstitutionId.Value,
            ProfilePictureUrl = model.ProfilePictureUrl.ToLower(),
            Department = model.Department.ToLower(),
            FacebookUrl = model.FacebookUrl.ToLower(),
            XUrl = model.XUrl.ToLower(),
            InstaUrl = model.InstaUrl.ToLower(),
            Bio =model.Bio.ToLower(),
            VerificationStatus = VerificationStatus.NotVerify,
            AcademicLevel = model.AcademicLevel
        };

        await _unitOfWork.Context.AddAsync(vendor);
        await _unitOfWork.CommitAsync();

        await _unitOfWork.Context.Database.ExecuteSqlRawAsync(ProfileSQL.SetAsVendor, new NpgsqlParameter("@id", user.Id));
        _notificationService.VendorOnboardingEmailAsync(user.Email, user.FirstName).Forget();

        return _responseService.SuccessResponse("vendor onbording was successful");
    }

    public async Task<ServiceResponse<string>> DeleteVendorAsync(Guid profileId)
    {
        var isProfileIdExist = await _unitOfWork.Context.Vendors.AnyAsync(x => x.ProfileId == profileId);
        if(!isProfileIdExist) return _responseService.ErrorResponse<string>("Invalid Request!");
           
        await _unitOfWork.Context.Database.ExecuteSqlRawAsync(VendorSQL.DeleteVendor, new NpgsqlParameter("@profileId", profileId));

        return _responseService.SuccessResponse("Vendor Deleted Successfully");
    }
    public async Task<ServiceResponse<PagedList<GetVendorDTO>>> GetAsync(int page = 1, int pageSize = 15)
    {   
        var experts = _unitOfWork.Context.Vendors
            .AsNoTracking()
            .Select(x => new GetVendorDTO
            {
                FullName = $"{x.Profile.FirstName} {x.Profile.LastName}",
                Email = x.Profile.Email,
                Role = x.Profile.Role,
                VendorInstitutionId = x.VendorInstitutionId,
                AcademicLevel = x.AcademicLevel,
                TotalAds = _unitOfWork.Context.Ads.Count(c => c.VendorId == x.Id),
                ProfilePictureUrl = x.ProfilePictureUrl
            });

        return await _responseService.PagedResponseAsync(experts, page, pageSize, "Vendors");
    }

    public async Task<ServiceResponse<GetVendorDTO>> GetDetailsAsync(Guid vendorId)
    {
        var vendor = await _unitOfWork.Context.Vendors
            .AsNoTracking()
            .Where(x => x.Id == vendorId)
            .Select(x => new GetVendorDTO
            {
                FullName = $"{x.Profile.FirstName} {x.Profile.LastName}",
                Email = x.Profile.Email,
                Bio = x.Bio,
                Role = x.Profile.Role,
                VendorInstitutionId = x.VendorInstitutionId,
                Department = x.Department,
                AcademicLevel = x.AcademicLevel,
                TotalAds = _unitOfWork.Context.Ads.Count(c => c.VendorId == x.Id),
                ProfilePictureUrl = x.ProfilePictureUrl,
                XUrl = x.XUrl,
                FacebookUrl = x.FacebookUrl,
                InstaUrl = x.InstaUrl
            }).FirstOrDefaultAsync();

            if(vendor is null) return _responseService.ErrorResponse<GetVendorDTO>("Invalid request");

            vendor.VendorReview = await _unitOfWork.Context.VendorReviews
            .AsNoTracking()
            .Where(x => x.VendorId == vendorId)
            .Select(x => new GetVendorReviewDTO
            {
                ReviewId = x.Id,
                ReviewerName = $"{x.Buyer.Profile.FirstName} {x.Buyer.Profile.LastName}",
                Review = x.Review
            })
            .ToListAsync();

        return _responseService.SuccessResponse(vendor);
    }
}