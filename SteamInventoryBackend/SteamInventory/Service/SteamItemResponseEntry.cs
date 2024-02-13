namespace SteamInventory.Service;

public class SteamItemResponseEntry
{
    public string? ItemName { get; set; }
    public string? Image { get; set; }
    public int ItemQuantity { get; set; }
    public double PriceLatest { get; set; }
    public string? ItemProfitable { get; set; }
    public List<SteamItemHistoryResponseEntry>? History { get; set; }
}