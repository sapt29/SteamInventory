namespace SteamInventory.Api.Models;

public class GetUserInventoryApiResponse
{
    public string? UserName { get; set; }
    public string? UserImage { get; set; }
    public GetUserInventoryApiResponseEntry[]? Items { get; set; }
}