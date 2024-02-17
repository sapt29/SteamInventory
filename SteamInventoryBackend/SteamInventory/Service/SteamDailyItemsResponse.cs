namespace SteamInventory.Service;

public class SteamDailyItemsResponse
{
    public string Id { get; set; }
    public int? Sold24h { get; set; }
    public double PriceLatestSell { get; set; }
    public double PriceReal24h { get; set; }
}
