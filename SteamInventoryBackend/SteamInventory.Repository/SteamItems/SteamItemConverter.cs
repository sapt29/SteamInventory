using SteamInventory.Models;

namespace SteamInventory.Repository.SteamItems;

internal static class SteamItemConverter
{
    internal static SteamItem ToSteamItem(this SteamItemMongoDb steamItemMongoDb)
    {
        return new SteamItem()
        {
            Id = steamItemMongoDb.Id,
            Image = steamItemMongoDb.Image,
            ItemHashName = steamItemMongoDb.ItemHashName,
            History = steamItemMongoDb.History?.ConvertAll(i => i.ToSteamItemHistory())
        };
    }
    internal static SteamItemHistory ToSteamItemHistory(this SteamItemHistoryMongoDb steamItemHistoryMongoDb)
    {
        return new SteamItemHistory()
        {
            Average = steamItemHistoryMongoDb.Average,
            Date = steamItemHistoryMongoDb.Date,
            //Date = DateTime.Parse(steamItemHistoryMongoDb.Date.ToString("yyyy-MM-dd")),
            ItemsSold = steamItemHistoryMongoDb.ItemsSold,
            LastPrice = steamItemHistoryMongoDb.LastPrice,
        };
    }

    internal static SteamItemHistoryMongoDb ToSteamItemHistoryMongoDb(this SteamItemHistory steamItemHistory) 
    {
        return new SteamItemHistoryMongoDb()
        {
            Average = steamItemHistory.Average,
            Date = steamItemHistory.Date,
            ItemsSold = steamItemHistory.ItemsSold,
            LastPrice = steamItemHistory.LastPrice,
        };
    }
    internal static SteamItemMongoDb ToSteamItemMongoDb(this SteamItem steamItem)
    {
        return new SteamItemMongoDb()
        {
            Id = steamItem.Id,
            Image = steamItem.Image,
            ItemHashName = steamItem.ItemHashName,
            History = steamItem.History?.ConvertAll(i => i.ToSteamItemHistoryMongoDb())
        };
    }
}
