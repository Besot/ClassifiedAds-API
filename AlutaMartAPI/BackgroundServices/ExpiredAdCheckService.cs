using AlutaMartAPI.Database;
using AlutaMartAPI.Services;
namespace AlutaMartAPI.BackgroundServices;
    public class ExpiredAdCheckService : BackgroundService
    {
        private readonly IAdsService _adsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExpiredAdCheckService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(12); // Set the interval for the job

        public ExpiredAdCheckService(IAdsService adsService, IUnitOfWork unitOfWork, ILogger<ExpiredAdCheckService> logger)
        {
            _adsService = adsService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExpiredAdCheckService is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("ExpiredAdCheckService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var batchSize = 1000; // Define batch size
                    var totalProcessed = 0;

                    while (true)
                    {
                        var response = await _adsService.SetIsFeaturedFalseForExpiredAdsAsync(batchSize);

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

                // Delay until the next execution
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("ExpiredAdCheckService has stopped.");
        }
    }
