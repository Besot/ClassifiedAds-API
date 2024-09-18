
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public interface IDashboardService
{
    Task<ServiceResponse<AdminDashboardDTO>> GetAnalyticsAsync();
}