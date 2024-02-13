namespace SteamInventory.Service;

public class UserInventoryHistorySteamResponse
{
    public DateTime Createdat { get; set; }
    public double Avg { get; set; }
    public int Sold { get; set; }
    public double Price { get; set; }
    public UserInventoryHistorySteamResponse()
    {
        Createdat = DateTime.MinValue;
        Avg = 0.0;
        Sold = 0;
        Price = 0.0;
    }
}
