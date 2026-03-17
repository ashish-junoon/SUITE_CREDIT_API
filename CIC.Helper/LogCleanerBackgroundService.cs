
using CIC.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CIC.Helper
{
    public class LogCleanerBackgroundService : BackgroundService
    {
        private readonly ILogger<LogCleanerBackgroundService> _logger;
        private readonly LogCleanerSettings _settings;
        public LogCleanerBackgroundService(ILogger<LogCleanerBackgroundService> logger, IOptions<LogCleanerSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var folder in _settings.Folders ?? [])
                    {
                        if (!Directory.Exists(folder))
                            continue;

                        var files = Directory.GetFiles(folder);

                        foreach (var file in files)
                        {
                            try
                            {
                                DateTime creation = File.GetCreationTime(file);

                                if ((DateTime.Now - creation).TotalDays > _settings.DaysToKeep)
                                {
                                    File.Delete(file);
                                    _logger.LogInformation($"Deleted old log: {file}");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Failed to delete: {file}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in log cleaner service");
                }

                // Run daily
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}
