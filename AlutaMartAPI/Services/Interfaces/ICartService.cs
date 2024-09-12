using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public interface ICartService
{
    Task<ServiceResponse<string>> AddToCartAsync(AddToCartDTO model, UserDTO user);
    Task<ServiceResponse<string>> DeleteCartAsync(Guid cartId, UserDTO user);
    Task<ServiceResponse<PagedList<GetCartDTO>>> GetCartByIdAsync(UserDTO user, int page = 1, int pageSize = 15);
    Task<ServiceResponse<string>> RemoveFromCartAsync(RemoveFromCartDTO model, UserDTO user);
}