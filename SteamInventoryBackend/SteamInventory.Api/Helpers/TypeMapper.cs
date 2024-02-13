using SteamInventory.Api.Models;
using SteamInventory.Service;

namespace SteamInventory.Api.Helpers;

public static class TypeMapper
{
    internal static GetUserInventoryApiResponse ToApi(GetUserInventoryResponse respose)
    {
        return new GetUserInventoryApiResponse()
        {
            UserName = respose.UserName,
            UserImage = respose.UserProfilePicture,
            Items = ToApi(respose.Items!).ToArray()
        };
    }

    internal static IEnumerable<GetUserInventoryApiResponseEntry> ToApi(IEnumerable<SteamItemResponseEntry> itemEntries)
    {
        foreach (var itemEntry in itemEntries)
        {
            yield return new GetUserInventoryApiResponseEntry()
            {
                ItemName = itemEntry.ItemName,
                ItemImage = itemEntry.Image,
                ItemQuantity = itemEntry.ItemQuantity,
                PriceLatest = itemEntry.PriceLatest,
                ItemProfitable = itemEntry.ItemProfitable,
                ItemsHistory = ToApi(itemEntry.History!).ToArray()
            };
        }
    }

    internal static IEnumerable<UserInventoryHistoryApiResponse> ToApi(IEnumerable<SteamItemHistoryResponseEntry> historyEntries)
    {
        foreach(var historyEntry in historyEntries)
        {
            yield return new UserInventoryHistoryApiResponse()
            {
                Average = historyEntry.Average,
                DatePrice = historyEntry.Date.ToString("MM-dd-yyyy"),
                ItemsSold = historyEntry.ItemsSold,
                LastPrice = historyEntry.LastPrice,
            };
        }
    }
}