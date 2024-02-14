using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SteamInventory.Helpers;
using SteamInventory.Interfaces;

namespace SteamInventory.BackgroundService;

public class DataBaseUpdateService : IHostedService, IDisposable
{
    private readonly ILogger<DataBaseUpdateService> _logger;
    private readonly ISteamItemsRepository _steamItemsRepository;
    private readonly ISteamWebApi _steamWebApi;
    public Timer _timer;
    public DataBaseUpdateService(ISteamItemsRepository steamItemsRepository, ILogger<DataBaseUpdateService> logger, ISteamWebApi steamWebApi) 
    {  
        _steamItemsRepository = steamItemsRepository;
        _steamWebApi = steamWebApi;
        _logger = logger;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Calculate the time until the next 2:30 am
        DateTime now = DateTime.Now;
        DateTime nextRunTime = new DateTime(now.Year, now.Month, now.Day, 2, 30, 0);
        if (now > nextRunTime)
        {
            nextRunTime = nextRunTime.AddDays(1);
        }

        // Set up the timer to run every 24 hours (1 day)
        TimeSpan timeUntilNextRun = nextRunTime - now;
        _timer = new Timer(async _ => await DailyUpdateDataBaseHistory(), null, timeUntilNextRun, TimeSpan.FromHours(24));

        return Task.CompletedTask;
    }
    public async Task DailyUpdateDataBaseHistory()
    {
        _logger.LogInformation("Starting Scheduled task");
        var itemIds = await _steamItemsRepository.GetSteamItemsIds();
        var allSteamItems = await _steamWebApi.GetAllItemsSteam();
        foreach (var item in allSteamItems)
        {
            if (itemIds.Contains(item.Id))
            {
                await _steamItemsRepository.AddHistoryToExistingItem(item.Id, TypeMapper.ToDomain(item));
            }
        }
        _logger.LogInformation("Finished Scheduled task");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    public void Dispose()
    {
        _timer?.Dispose();
    }
}
