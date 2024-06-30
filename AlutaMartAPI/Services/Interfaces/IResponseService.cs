using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public interface IResponseService
{
    ServiceResponse<T> ErrorResponse<T>(string message);

    ServiceResponse<T> SuccessResponse<T>(T data, string message = null);

    ServiceResponse<string> SuccessResponse(string message);

    ServiceResponse<T> UserErrorResponse<T>(T data, string message = null);

    ServiceResponse<T> ExceptionResponse<T>(Exception exception) where T : class;

    Task<ServiceResponse<PagedList<T>>> PagedResponseAsync<T>(IQueryable<T> source,
        int page, int pageSize, string entityName) where T : class;
}