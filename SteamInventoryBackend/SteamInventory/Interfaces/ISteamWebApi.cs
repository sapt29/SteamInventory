using SteamInventory.Service;

namespace SteamInventory.Interfaces;

public interface ISteamWebApi
{
    Task<IEnumerable<SteamDailyItemsResponse>> GetAllItemsSteam();
    Task<IEnumerable<UserInventoryHistorySteamResponse>> GetHistoryItemSteam(string itemName);
    Task<UserInfoSteamResponse> GetUserInfoSteam(string userId);
    Task<IEnumerable<UserInventoryItemsSteamResponse>> GetUserInventoryItemsSteamByUserId(string userId);
}
