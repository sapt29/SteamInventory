using SteamInventory.Service;

namespace SteamInventory.Interfaces;

public interface IMainService
{
    StartupInfo StartupInfo { get; }
    Task<GetUserInventoryResponse> GetUserInventoryByIdAsync(string userSteamId);
}
