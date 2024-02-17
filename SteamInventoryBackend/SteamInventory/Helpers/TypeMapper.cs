using SteamInventory.Models;
using SteamInventory.Service;

namespace SteamInventory.Helpers;

public static class TypeMapper
{
    internal static SteamItem ToDomain(UserInventoryItemsSteamResponse item, IEnumerable<UserInventoryHistorySteamResponse> itemHistory)
    {
        return new SteamItem()
        {
            Id = item.Id,
            Image = item.Image,
            ItemHashName = item.Markethashname,
            History = ToDomain(itemHistory).OrderBy(x => x.Date).ToList()
        };
    }
    
    internal static IEnumerable<SteamItemHistory> ToDomain(IEnumerable<UserInventoryHistorySteamResponse> itemHistory)
    {
        foreach (var item in itemHistory) 
        {
            yield return new SteamItemHistory()
            {
                Average = item.Avg,
                Date = item.Createdat,
                ItemsSold = item.Sold,
                LastPrice = item.Price,
            };
        }
    }

    internal static SteamItemHistory ToDomain(SteamDailyItemsResponse itemHistory)
    {
       return new SteamItemHistory()
        {
            Average = itemHistory.PriceReal24h,
            Date = DateTime.UtcNow,
            ItemsSold = itemHistory?.Sold24h ?? 0,
            LastPrice = itemHistory?.PriceLatestSell ?? 0,
        };
    }
    internal static IEnumerable<SteamItemHistoryResponseEntry> ToResponse(IEnumerable<SteamItemHistory> itemHistory)
    {
        foreach (var item in itemHistory) 
        {
            yield return new SteamItemHistoryResponseEntry()
            {
                Average = item.Average,
                Date = item.Date,
                ItemsSold = item.ItemsSold,
                LastPrice = item.LastPrice,
            };
        }
    }

    internal static IEnumerable<SteamItemResponseEntry> ToResponse(Dictionary<string, SteamItem> stemItems, IEnumerable<UserInventoryItemsSteamResponse> steamItemsInventory)
    {
        foreach(var itemInventory in steamItemsInventory)
        {
            stemItems.TryGetValue(itemInventory.Id!, out var stemItem);
            var historyItem = stemItem!.History;
            yield return new SteamItemResponseEntry()
            {
                Image = itemInventory.Image,
                ItemQuantity = itemInventory.Count,
                ItemName = itemInventory.Markethashname,
                PriceLatest = itemInventory.Pricelatest,
                ItemProfitable = ProfitableItem.GetProfitable(historyItem!),
                History = ToResponse(historyItem!).OrderByDescending(x => x.Date).ToList()
            };
        }
    }
}
