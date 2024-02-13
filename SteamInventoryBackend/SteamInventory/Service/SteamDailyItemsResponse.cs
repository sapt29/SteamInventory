namespace SteamInventory.Service;

public class SteamDailyItemsResponse
{
    public string Id { get; set; }
    public int? Sold24h { get; set; }
    public double Pricelatestsell { get; set; }
    public double Pricesafe24h { get; set; }
}
