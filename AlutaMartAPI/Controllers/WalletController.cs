using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;


namespace AlutaMartAPI.Controllers;
    public class WalletController(IWalletService walletService) :BaseController
    {
        [HttpGet, AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<GetWalletDTO>), statusCode: 200)]
        public async Task<IActionResult> GetWallet() => Ok(await walletService.GetWalletAsync(CurrentUser));
    }