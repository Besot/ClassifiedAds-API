
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface ICategoryService
{
    Task<ServiceResponse<List<GetAdsCategoryDTO>>>  GetAsync();
}