using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SteamInventory.Repository.SteamItems;

public class SteamItemMongoDb
{
    [BsonId]
    public string? Id { get; set; }

    [BsonElement("ItemHashName")]
    public string? ItemHashName { get; set; }

    [BsonElement("Image")]
    public string? Image { get; set; }

    [BsonElement("History")]
    public List<SteamItemHistoryMongoDb>? History { get; set; }

}
