using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using System;
using System.Threading.Tasks;

namespace AlutaMartAPI.Services
{
    public interface IReportService
    {
        Task<ServiceResponse<string>> CreateReportAsync(CreateReportDTO model, UserDTO currentUser);
        Task<ServiceResponse<PagedList<GetReportDTO>>> GetReportsAsync(int page = 1, int pageSize = 20);
        Task<ServiceResponse<GetReportDTO>> GetReportDetailsAsync(Guid reportId);
        Task<ServiceResponse<string>> UpdateReportStatusAsync(Guid reportId, UpdateReportStatusDTO model, UserDTO currentUser);
    }
}
