using SteamInventory.Models;

namespace SteamInventory.Interfaces;

public interface ISteamItemsRepository
{
    Task<IEnumerable<SteamItem>> GetSteamItemsAsync();
    Task AddSteamItem(SteamItem steamItem);
    Task AddHistoryToExistingItem(string itemId, SteamItemHistory itemHistory);
    Task AddMultipleSteamItems(List<SteamItem> steamItems);
    Task<IEnumerable<SteamItem>> GetSteamItemsByIdsAsync(IEnumerable<string> itemIds);
    Task<IEnumerable<string>> GetSteamItemsIds();
}
