namespace SteamInventory.Api.Models;

public class UserInventoryHistoryApiResponse
{
    public string DatePrice { get; set; }
    public double Average { get; set; }
    public int ItemsSold { get; set; }
    public double LastPrice { get; set; }
}