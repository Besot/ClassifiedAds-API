using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlutaMartAPI.Services;

public class ResponseService : IResponseService
{
    public ServiceResponse<T> ErrorResponse<T>(string message) => new() { Message = message ?? "An error occured" };
    public ServiceResponse<T> ExceptionResponse<T>(Exception exception) where T : class => new() { Message = exception.Message };

    public ServiceResponse<T> UserErrorResponse<T>(T data, string message = null)
        => new()
        {
            Message = message ?? "An error occured",
            Data = data
        };

    public ServiceResponse<T> SuccessResponse<T>(T data, string message = null)
        => new()
        {
            Message = message ?? "Operation successful",
            Status = true,
            Data = data
        };

    public ServiceResponse<string> SuccessResponse(string message)
        => new()
        {
            Message = message,
            Status = true,
            Data = message
        };

    public async Task<ServiceResponse<PagedList<T>>> PagedResponseAsync<T>(IQueryable<T> source, int page, int pageSize, string entityName) where T : class
    {
        page  = page < 1 ? 1 : page;
        pageSize  = pageSize < 1 ? 20 : pageSize;
        pageSize = pageSize > 100 ? pageSize : 100;
        var count = await source.CountAsync();
        var items = count > 0 ? await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync() : [];
        var pagedResp = new PagedList<T>(items, count, page, pageSize, entityName);
        return new ServiceResponse<PagedList<T>>
        {
            Message = pagedResp.Summary,
            Status = true,
            Data = pagedResp
        };
    }
}