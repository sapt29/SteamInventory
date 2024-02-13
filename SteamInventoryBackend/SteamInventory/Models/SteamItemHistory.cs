namespace SteamInventory.Models;

public class SteamItemHistory
{
    public DateTime Date { get; set; }
    public double Average { get; set; }
    public int ItemsSold { get; set; }
    public double LastPrice { get; set; }
}