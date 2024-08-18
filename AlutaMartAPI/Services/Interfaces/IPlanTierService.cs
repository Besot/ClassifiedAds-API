
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IPlanTierService
{
    Task<ServiceResponse<List<GetPlanTierDTO>>> GetAsync();
}
