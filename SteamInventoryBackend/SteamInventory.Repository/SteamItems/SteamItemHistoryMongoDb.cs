using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SteamInventory.Repository.SteamItems;

public class SteamItemHistoryMongoDb
{
    [BsonElement("Date")]
    //[BsonDateTimeOptions(DateOnly = true)]
    public DateTime Date { get; set; }

    [BsonElement("Average")]
    public double Average { get; set; }

    [BsonElement("ItemsSold")]
    public int ItemsSold { get; set; }

    [BsonElement("LastPrice")]
    public double LastPrice { get; set; }
}
