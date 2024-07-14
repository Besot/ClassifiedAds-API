
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IInstitutionService
{
    Task<ServiceResponse<List<GetInstitutionDTO>>> GetAsync();
}
