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
        if(model.InstitutionId == null || !model.InstitutionId.HasValue) return _responseService.ErrorResponse<string>("Vendor's institution is required");

        var isAlreadyVendor = await _unitOfWork.Context.Vendors.AnyAsync(x => x.ProfileId == user.Id);
        if(isAlreadyVendor) 
        {
            _notificationService.VendorOnboardingEmailAsync(user.Email, user.FirstName).Forget();
            return _responseService.SuccessResponse("User is already a vendor");
        }

        var validInstitution = await _unitOfWork.Context.Institutions.AnyAsync(x => x.Id == model.InstitutionId.Value);
        if(!validInstitution) return _responseService.ErrorResponse<string>("Invalid request");

        if (!Guid.TryParse(Constants.PlanTierFreePlanId, out Guid planTierId))return _responseService.ErrorResponse<string>("Invalid PlanTierId format");


        var vendor = new Vendor
        {
            ProfileId = user.Id,
            InstitutionId = model.InstitutionId.Value,
            PlanTierId = planTierId,
            VendorPictureUrl = model.ProfilePictureUrl.ToLower(),
            BrandName = model.BrandName.ToLower(),
            FacebookUrl = model.FacebookUrl.ToLower(),
            XUrl = model.XUrl.ToLower(),
            InstaUrl = model.InstaUrl.ToLower(),
            Bio =model.Bio.ToLower(),
            VerificationStatus = VerificationStatus.NotVerify,
        };

        await _unitOfWork.Context.AddAsync(vendor);

        var nin = new IdentityCard
        {
            ProfileId = user.Id,
            NIN = model.NIN
        };
        await _unitOfWork.Context.IdentityCards.AddAsync(nin);
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
        var vendors = _unitOfWork.Context.Vendors
            .AsNoTracking()
            .Select(x => new GetVendorDTO
            {
                BrandName = x.BrandName,
                FullName = $"{x.Profile.FirstName} {x.Profile.LastName}",
                Email = x.Profile.Email,
                Role = x.Profile.Role,
                VendorInstitutionId = x.InstitutionId,
                TotalAds = _unitOfWork.Context.Ads.Count(c => c.VendorId == x.Id),
                ProfilePictureUrl = x.VendorPictureUrl
            });

        return await _responseService.PagedResponseAsync(vendors, page, pageSize, "Vendors");
    }

    public async Task<ServiceResponse<GetVendorDTO>> GetDetailsAsync(Guid vendorId)
    {
        var vendor = await _unitOfWork.Context.Vendors
            .AsNoTracking()
            .Where(x => x.Id == vendorId)
            .Select(x => new GetVendorDTO
            {
                FullName = $"{x.Profile.FirstName} {x.Profile.LastName}",
                BrandName = x.BrandName,
                Email = x.Profile.Email,
                Bio = x.Bio,
                Role = x.Profile.Role,
                VendorInstitutionId = x.InstitutionId,
                VerificationStatus = x.VerificationStatus,
                NIN = x.NIN,
                TotalAds = _unitOfWork.Context.Ads.Count(c => c.VendorId == vendorId),
                NumberOfReviews = _unitOfWork.Context.Reviews.Count(c => c.VendorId == vendorId),
                ProfilePictureUrl = x.VendorPictureUrl,
                XUrl = x.XUrl,
                FacebookUrl = x.FacebookUrl,
                InstaUrl = x.InstaUrl
            }).FirstOrDefaultAsync();

            if(vendor is null) return _responseService.ErrorResponse<GetVendorDTO>("Invalid request");

            vendor.VendorReview = await _unitOfWork.Context.Reviews
            .AsNoTracking()
            .Where(x => x.VendorId == vendorId)
            .Select(x => new GetVendorReviewDTO
            {
                ReviewId = x.Id,
                ReviewerName = $"{x.Buyer.Profile.FirstName} {x.Buyer.Profile.LastName}",
                Review = x.Content,
                ReviewerPicture = x.Buyer.Profile.ProfilePictureUrl,
                ReviewDate = x.Modified,
                Edited = x.Modified != x.Created
            })
            .ToListAsync();

        return _responseService.SuccessResponse(vendor);
    }
}