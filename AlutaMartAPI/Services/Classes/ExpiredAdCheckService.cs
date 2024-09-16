using AlutaMartAPI.Database;
using AlutaMartAPI.Services;
using AlutaMartAPI.SQLQueries;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class ExpiredAdCheckService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ExpiredAdCheckService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(12); // Set the interval for the job

    public ExpiredAdCheckService(IServiceScopeFactory serviceScopeFactory, ILogger<ExpiredAdCheckService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExpiredAdCheckService is starting.");

        stoppingToken.Register(() =>
            _logger.LogInformation("ExpiredAdCheckService is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var responseService = scope.ServiceProvider.GetRequiredService<IResponseService>();

                try
                {
                    var batchSize = 1000; // Define batch size
                    var totalProcessed = 0;

                    while (true)
                    {
                        var response = await SetIsFeaturedFalseForExpiredAdsAsync(unitOfWork, responseService, batchSize);

                        if (!response.Status)
                        {
                            _logger.LogError("Failed to update expired ads: {Message}", response.Message);
                            break;
                        }

                        var processedCount = response.Data; // Assume response.Data contains the count of processed ads
                        totalProcessed += processedCount;

                        if (processedCount < batchSize)
                        {
                            break; // Exit loop if the last batch was smaller than the batch size
                        }
                    }

                    _logger.LogInformation("Processed {TotalProcessed} expired ads.", totalProcessed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during expired ad check.");
                }
            }

            // Delay until the next execution
            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("ExpiredAdCheckService has stopped.");
    }

    private static async Task<ServiceResponse<int>> SetIsFeaturedFalseForExpiredAdsAsync(IUnitOfWork unitOfWork, IResponseService responseService, int batchSize)
    {
        try
        {
            var affectedRows = await unitOfWork.Context.Database.ExecuteSqlRawAsync(AdSQL.SetIsFeaturedFalseForExpiredAds, new NpgsqlParameter("@batchSize", batchSize));
            await unitOfWork.CommitAsync();
            return responseService.SuccessResponse<int>(affectedRows, "Successfully updated expired featured ads.");
        }
        catch (Exception ex)
        {
            return responseService.ErrorResponse<int>($"Error updating expired featured ads: {ex.Message}");
        }
    }
}
