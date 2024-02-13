using Microsoft.Extensions.Logging;
using SteamInventory.Helpers;
using SteamInventory.Interfaces;
using SteamInventory.Models;

namespace SteamInventory.Service;

public class MainService : IMainService
{
    private readonly ISteamItemsRepository _steamItemsRepository;
    private readonly ISteamWebApi _steamWebApi;
    private readonly ILogger<MainService> _logger;

    public StartupInfo StartupInfo { get; init; }
    public MainService(ISteamItemsRepository steamItemsRepository, ISteamWebApi steamWebApi, ILogger<MainService> logger) 
    {
        StartupInfo = new StartupInfo
        {
            Status = ServiceStatus.Initialized
        };
        _steamItemsRepository = steamItemsRepository;
        _steamWebApi = steamWebApi;
        _logger = logger;
    }

    public async Task<GetUserInventoryResponse> GetUserInventoryByIdAsync(string userId)
    {
        try
        {
            var userProfile = await GetUserInfoSteam(userId);
            var userInventoryItems = await GetUserInventoryItemsSteamByUserId(userId);
            var steamItemsDb = await GetSteamItemsByIdsDb(userInventoryItems);
            var notExistingItems = GetNotExistingItemsInDb(userInventoryItems, steamItemsDb);
            if (notExistingItems?.Any() == true)
            {
                await AddNotExistingItemsToDb(notExistingItems);
            }
            var steamItemsDbDictionary = (await GetSteamItemsByIdsDb(userInventoryItems!)).ToDictionary(item => item.Id!);
            return new GetUserInventoryResponse()
            {
                UserProfilePicture = userProfile?.AvatarFull,
                UserName = userProfile?.PersonaName,
                Items = TypeMapper.ToResponse(steamItemsDbDictionary!, userInventoryItems!).ToList()
            };
        }
        catch (Exception ex) 
        {
            _logger.LogError("GetUserInventoryByIdAsync: Error  {}", ex.Message);
            throw;
        }
    }

    private async Task AddNotExistingItemsToDb(IEnumerable <UserInventoryItemsSteamResponse> notExistingItems)
    {
        List<SteamItem> newItems = [];
        int count = 0;
        foreach (var item in notExistingItems)
        {
            if (count == 10)
            {
                Thread.Sleep(62000);
                count = 0;
            }
            var newItem = TypeMapper.ToDomain(item, await GetHistoryItemSteam(item.Markethashname!));
            newItems.Add(newItem);
            count++;
        }
        await _steamItemsRepository.AddMultipleSteamItems(newItems);
    }

    private async Task<IEnumerable<SteamItem>> GetSteamItemsByIdsDb(IEnumerable<UserInventoryItemsSteamResponse> userInventoryItems)
    {
        return await _steamItemsRepository.GetSteamItemsByIdsAsync(userInventoryItems.Select(a => a.Id!));
    }

    private static List<UserInventoryItemsSteamResponse>? GetNotExistingItemsInDb(IEnumerable<UserInventoryItemsSteamResponse> userInventoryItems, IEnumerable<SteamItem> steamItemsDb)
    {
        return userInventoryItems?.Where(a => !steamItemsDb.Any(b => b.Id == a.Id)).ToList();
    }

    // Get Name and profilePic
    private async Task<UserInfoSteamResponse> GetUserInfoSteam(string userId)
    {
        try
        {
            return await _steamWebApi.GetUserInfoSteam(userId);
        }
        catch
        {
            throw;
        }
    }

    // Get User Steam Inventory
    private async Task<IEnumerable<UserInventoryItemsSteamResponse>> GetUserInventoryItemsSteamByUserId(string userId)
    {
        try
        {
            return await _steamWebApi.GetUserInventoryItemsSteamByUserId(userId);
        }
        catch
        {
            throw;
        }
    }

    // Get history of an item
    private async Task<IEnumerable<UserInventoryHistorySteamResponse>> GetHistoryItemSteam(string itemName)
    {
        try
        {
            return await _steamWebApi.GetHistoryItemSteam(itemName);
        }
        catch
        {
            throw;
        }
    }
}
