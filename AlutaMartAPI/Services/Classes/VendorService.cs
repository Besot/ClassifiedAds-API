using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.SQLQueries;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AlutaMartAPI.Services;
public class VendorService(IUnitOfWork _unitOfWork, IResponseService _responseService, INotificationService _notificationService, UserManager<Profile> userManager) : IVendorService
{
    public async Task<ServiceResponse<string>> CreateAsync(CreateVendorDTO model, UserDTO user)
    {
        if(model.VendorInstitutionId == null || !model.VendorInstitutionId.HasValue) return _responseService.ErrorResponse<string>("Expert's industry is required");

        var isAlreadyVendor = await _unitOfWork.Context.Vendors.AnyAsync(x => x.ProfileId == user.Id);
        if(isAlreadyVendor) 
        {
            _notificationService.VendorOnboardingEmailAsync(user.Email, user.FirstName).Forget();
            return _responseService.SuccessResponse("User is already an expert");
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

        await _unitOfWork.Context.Database.ExecuteSqlRawAsync(ProfileSQL.SetAsExpert, new NpgsqlParameter("@id", user.Id));
        _notificationService.VendorOnboardingEmailAsync(user.Email, user.FirstName).Forget();

        return _responseService.SuccessResponse("vendor onbording was successful");
    }
}