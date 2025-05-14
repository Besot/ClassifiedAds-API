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
    public async Task<ServiceResponse<string>> AddBankAccountAsync(CreateBankAccountDTO createBankAccountDTO, UserDTO currentUser)
    {

        if (!Constants.AcceptedCurrencyCode.Contains(createBankAccountDTO.Currency.ToLower().Trim()))
        {
            return _responseService.ErrorResponse<string>("Currency not accepted yet.");
        }

        var currency = await _unitOfWork.Context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == createBankAccountDTO.Currency);

        if (currency == null)
        {
            return _responseService.ErrorResponse<string>("Currency not found in the database.");
        }

        var existingBankAccount = await _unitOfWork.Context.BankAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ProfileId == currentUser.Id && b.BankAccountNumber == createBankAccountDTO.BankAccountNumber);

        if (existingBankAccount != null)
        {
            return _responseService.ErrorResponse<string>("This bank account already exists.");
        }

        var bankAccountCount = await _unitOfWork.Context.BankAccounts
            .AsNoTracking()
            .CountAsync(b => b.ProfileId == currentUser.Id);

        if (bankAccountCount >= 10)
        {
            return _responseService.ErrorResponse<string>("You cannot have more than 10 bank accounts.");
        }

        var userSecurityQuestion = await _unitOfWork.Context.SecurityQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(sq => sq.ProfileId == currentUser.Id);

        if (userSecurityQuestion == null)
        {
            return _responseService.ErrorResponse<string>("Security question not set for the user.");
        }

        var hashedAnswer = AppUtilities.HashString(createBankAccountDTO.SecurityQuestionAnswer);

        if (!string.Equals(userSecurityQuestion.SecurityQuestionAnswer, hashedAnswer, StringComparison.Ordinal))
        {
            return _responseService.ErrorResponse<string>("Security question answer is incorrect.");
        }

        var bankAccount = new BankAccount
        {
            ProfileId = currentUser.Id,
            CurrencyId = currency.Id,
            BankAccountName = createBankAccountDTO.BankAccountName,
            BankAccountNumber = createBankAccountDTO.BankAccountNumber,
            BankName = createBankAccountDTO.BankName,
            BankCode = createBankAccountDTO.BankCode
        };

        await _unitOfWork.Context.BankAccounts.AddAsync(bankAccount);
        await _unitOfWork.CommitAsync();

        return _responseService.SuccessResponse("Bank account created successfully.");
    }

    public async Task<ServiceResponse<string>> AddSecurityQuestionAsync(AddSecurityQuestionDTO addSecurityQuestionDTO, UserDTO currentUser)
    {
        var profile = await _unitOfWork.Context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == currentUser.Id);

        if (profile == null)
        {
            return _responseService.ErrorResponse<string>("Expert not found.");
        }

        var userSecurityQuestionCount = await _unitOfWork.Context.SecurityQuestions
            .CountAsync(sq => sq.ProfileId == currentUser.Id);

        if (userSecurityQuestionCount > 0)
        {
            return _responseService.ErrorResponse<string>("You can only have one security question.");
        }

        var existingSecurityQuestion = await _unitOfWork.Context.SecurityQuestions
            .FirstOrDefaultAsync(sq => sq.ProfileId == currentUser.Id && sq.SecurityQuestionKey == addSecurityQuestionDTO.SecurityQuestionKey);

        if (existingSecurityQuestion != null)
        {
            return _responseService.ErrorResponse<string>("This security question has already been added.");
        }

        var hashedAnswer = AppUtilities.HashString(addSecurityQuestionDTO.SecurityQuestionAnswer);
        var securityQuestion = new SecurityQuestion
        {
            ProfileId = currentUser.Id,
            SecurityQuestionKey = addSecurityQuestionDTO.SecurityQuestionKey,
            SecurityQuestionAnswer = hashedAnswer
        };

        await _unitOfWork.Context.SecurityQuestions.AddAsync(securityQuestion);
        await _unitOfWork.CommitAsync();

        return _responseService.SuccessResponse("Security question added successfully.");
    }

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



        var vendor = new Vendor
        {
            ProfileId = user.Id,
            InstitutionId = model.InstitutionId.Value,
            VendorPictureUrl = model.ProfilePictureUrl.ToLower(),
            BrandName = model.BrandName.ToLower(),
            FacebookUrl = model.FacebookUrl.ToLower(),
            XUrl = model.XUrl.ToLower(),
            InstaUrl = model.InstaUrl.ToLower(),
            Bio =model.Bio.ToLower(),
            VerificationStatus = VerificationStatus.NotVerify,
        };

        await _unitOfWork.Context.AddAsync(vendor);

       var freeTier = _unitOfWork.Context.PlanTiers.FirstOrDefault(pt => pt.Name == "free tier");

        if (freeTier != null)
        {
            var vendorPlan = new VendorPlanTier
            {
                ProfileId = user.Id,
                VendorId = vendor.Id,
                PlanTierId = freeTier.Id, // Set the PlanTierId to the Id of the "free tier"
                ExpiryDate = null // Set expiry date to null for free plan
            };

            // Save the vendor plan
            await _unitOfWork.Context.VendorPlan.AddAsync(vendorPlan);
        }
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

    public async Task<ServiceResponse<string>> CreateTransactionPINAsync(CreateTransactionPINDTO createTransactionPINDTO, UserDTO currentUser)
    {
        if (!string.Equals(createTransactionPINDTO.PIN, createTransactionPINDTO.PINConfirmation, StringComparison.Ordinal))
        {
            return _responseService.ErrorResponse<string>("PIN and PIN confirmation do not match.");
        }

        var profile = await _unitOfWork.Context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == currentUser.Id);

        if (profile == null)
        {
            return _responseService.ErrorResponse<string>("Expert not found.");
        }

        if (!string.IsNullOrEmpty(profile.TransactionPIN))
        {
            return _responseService.ErrorResponse<string>("Transaction PIN has already been set.");
        }

        var hashedPIN = AppUtilities.HashString(createTransactionPINDTO.PIN);

        profile.TransactionPIN = hashedPIN;
        _unitOfWork.Context.Profiles.Update(profile);
        await _unitOfWork.CommitAsync();

        return _responseService.SuccessResponse("Transaction PIN set successfully.");

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

       public async Task<ServiceResponse<GetBankAccountsDTO>> GetBankAccounts(UserDTO currentUser)
    {
        var vendor = await _unitOfWork.Context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == currentUser.Id);

        if (vendor == null)
        {
            return _responseService.ErrorResponse<GetBankAccountsDTO>("Expert not found.");
        }

        bool HasSetTransactionPIN = !string.IsNullOrEmpty(vendor.TransactionPIN);

        var userSecurityQuestion = await _unitOfWork.Context.SecurityQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(sq => sq.ProfileId == currentUser.Id);

        var HasSetSecurityQuestion = userSecurityQuestion != null;

        var bankAccounts = await _unitOfWork.Context.BankAccounts
            .AsNoTracking()
            .Where(b => b.ProfileId == currentUser.Id)
            .ToListAsync();

        return _responseService.SuccessResponse(new GetBankAccountsDTO
        {
            BankAccounts = bankAccounts,
            HasSetSecurityQuestion = HasSetSecurityQuestion,
            HasSetTransactionPIN = HasSetTransactionPIN,
            SecurityQuestion = HasSetSecurityQuestion ? userSecurityQuestion.SecurityQuestionKey : null,
        });
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

        if (vendor is null) return _responseService.ErrorResponse<GetVendorDTO>("Invalid request");

        vendor.VendorReview = await _unitOfWork.Context.Reviews
        .AsNoTracking()
        .Where(x => x.VendorId == vendorId)
        .Select(x => new GetVendorReviewDTO
        {
            ReviewId = x.Id,
            ReviewerName = $"{x.Profile.FirstName} {x.Profile.LastName}",
            Review = x.Content,
            ReviewerPicture = x.Profile.ProfilePictureUrl,
            ReviewDate = x.Modified,
            Edited = x.Modified != x.Created
        })
        .ToListAsync();

        return _responseService.SuccessResponse(vendor);
    }
}