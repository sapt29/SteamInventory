using SteamInventory.Models;

namespace SteamInventory.Interfaces;

public interface ISteamItemsRepository
{
    Task<IEnumerable<SteamItem>> GetSteamItemsByIdsAsync(IEnumerable<string> itemIds);
    Task AddHistoryToExistingItem(string itemId, SteamItemHistory itemHistory);
    Task<IEnumerable<string>> GetSteamItemsIds();
    Task AddMultipleSteamItems(List<SteamItem> steamItems);
    Task<IEnumerable<SteamItem>> GetSteamItemsAsync();
    Task AddSteamItem(SteamItem steamItem);
}
