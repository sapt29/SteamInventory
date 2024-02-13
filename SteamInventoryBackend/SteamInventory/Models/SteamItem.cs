namespace SteamInventory.Models;

public class SteamItem
{
    public string? Id { get; set; }
    public string? ItemHashName { get; set; }
    public string? Image { get; set; }
    public List<SteamItemHistory>? History { get; set; }
}
