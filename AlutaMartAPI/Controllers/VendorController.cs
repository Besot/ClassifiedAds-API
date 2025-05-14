using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class VendorController(IVendorService _vendorService) :BaseController
    {
        [HttpPost("Create")]
	    [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	    public async Task<IActionResult> Create([FromBody] CreateVendorDTO model) => Ok(await _vendorService.CreateAsync(model, CurrentUser));
   
        [HttpGet, AccessControl([Roles.SuperAdmin, Roles.AdminUser, Roles.SuperAdmin], AccessType.Allow)]
        [ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetVendorDTO>>), statusCode: 200)]
        public async Task<IActionResult> GetVendor(int page = 1, int pageSize = 15) => Ok(await _vendorService.GetAsync( page, pageSize));

        [HttpGet("Details/{vendorId}"), AccessControl([Roles.SuperAdmin, Roles.AdminUser, Roles.SuperAdmin, Roles.Vendor], AccessType.Allow)]
        [ProducesResponseType(type: typeof(ServiceResponse<GetVendorDTO>), statusCode: 200)]
        public async Task<IActionResult> GetDetails(Guid vendorId) => Ok(await _vendorService.GetDetailsAsync(vendorId));

        [HttpDelete("Delete/{profileId}"), AccessControl([Roles.AdminUser, Roles.SuperAdmin], AccessType.Allow)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> DeleteVendor(Guid profileId)  => Ok(await _vendorService.DeleteVendorAsync(profileId));

        [HttpPost("Add-Bank-Account"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> AddBankAccount([FromBody] CreateBankAccountDTO model)
            => Ok(await _vendorService.AddBankAccountAsync(model, CurrentUser));

        [HttpGet("Bank-Accounts"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<GetBankAccountsDTO>), statusCode: 200)]
        public async Task<IActionResult> GetExpertBankAccounts()
            => Ok(await _vendorService.GetBankAccounts(CurrentUser));
        
        [HttpPost("Create-Transaction-PIN"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> CreateTransactionPIN([FromBody] CreateTransactionPINDTO model)
            => Ok(await _vendorService.CreateTransactionPINAsync(model, CurrentUser));
            
        [HttpPost("Add-Security-Question"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> AddSecurityQuestion([FromBody] AddSecurityQuestionDTO model)
            => Ok(await _vendorService.AddSecurityQuestionAsync(model, CurrentUser));
    }