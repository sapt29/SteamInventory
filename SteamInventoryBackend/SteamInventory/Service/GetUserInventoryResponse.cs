using SteamInventory.Models;

namespace SteamInventory.Service;

public class GetUserInventoryResponse
{
    public string? UserName { get; set; }
    public string? UserProfilePicture { get; set; }
    public List<SteamItemResponseEntry>? Items { get; set; }
}