namespace SteamInventory.Service;

public class SteamItemHistoryResponseEntry
{
    public DateTime Date { get; set; }
    public double Average { get; set; }
    public int ItemsSold { get; set; }
    public double LastPrice { get; set; }
}