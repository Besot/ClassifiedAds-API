using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using System;
using System.Threading.Tasks;

namespace AlutaMartAPI.Services
{
    public interface IWishlistService
    {
        Task<ServiceResponse<GetWishlistDTO>> GetWishlistAsync(UserDTO currentUser);
        Task<ServiceResponse<string>> AddToWishlistAsync(AddToWishlistDTO model, UserDTO currentUser);
        Task<ServiceResponse<string>> RemoveFromWishlistAsync(Guid wishlistItemId, UserDTO currentUser);
        Task<ServiceResponse<string>> ClearWishlistAsync(UserDTO currentUser);
    }
}
