using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public interface ICartService
{
    Task<ServiceResponse<string>> AddToCartAsync(AddToCartDTO model, UserDTO user);
}