namespace SteamInventory.Api.Models;

public class GetUserInventoryApiResponseEntry
{
    public string? ItemName { get; set; }
    public string? ItemImage { get; set; }
    public int ItemQuantity { get; set; }
    public double PriceLatest { get; set; }
    public string? ItemProfitable { get; set; }
    // Dates & info for graph, figure out how to do it
    public UserInventoryHistoryApiResponse[] ItemsHistory { get; set; } 
}