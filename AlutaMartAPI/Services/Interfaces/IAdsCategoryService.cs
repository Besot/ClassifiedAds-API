
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IAdsCategoryService
{
    Task<ServiceResponse<List<GetAdsCategoryDTO>>>  GetAsync();
}