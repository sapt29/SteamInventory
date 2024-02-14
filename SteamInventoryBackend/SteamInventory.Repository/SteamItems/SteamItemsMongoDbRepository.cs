using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SteamInventory.Interfaces;
using SteamInventory.Models;

namespace SteamInventory.Repository.SteamItems;

public class SteamItemsMongoDbRepository : ISteamItemsRepository
{
    private readonly IMongoCollection<SteamItemMongoDb> _steamItemsCollection;

    public SteamItemsMongoDbRepository(IOptions<MongoDbConfiguration> mongoDbConfiguration)
    {
        var mongoClient = new MongoClient(mongoDbConfiguration.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbConfiguration.Value.DatabaseName);
        _steamItemsCollection = mongoDatabase.GetCollection<SteamItemMongoDb>(mongoDbConfiguration.Value.CollectionName);
    }

    public async Task<IEnumerable<SteamItem>> GetSteamItemsAsync()
    {
        var steamItemsMongoDb = await _steamItemsCollection.Find(filter => true).ToListAsync();
        return steamItemsMongoDb.ConvertAll(i => i.ToSteamItem());
    }

    public async Task<IEnumerable<SteamItem>> GetSteamItemsByIdsAsync(IEnumerable<string> itemIds)
    {
        var steamItemsMongoDb =  await _steamItemsCollection.AsQueryable().Where(x => itemIds.Contains(x.Id!)).ToListAsync();
        return steamItemsMongoDb.ConvertAll(i => i.ToSteamItem());
    }

    public async Task AddSteamItem(SteamItem steamItem)
    {
        await _steamItemsCollection.InsertOneAsync(steamItem.ToSteamItemMongoDb());
    }

    public async Task AddHistoryToExistingItem(string itemId, SteamItemHistory itemHistory)
    {  
        await _steamItemsCollection.UpdateOneAsync(x => x.Id == itemId, Builders<SteamItemMongoDb>.Update.AddToSet(z => z.History, itemHistory.ToSteamItemHistoryMongoDb()));
    }

    public async Task AddMultipleSteamItems(List<SteamItem> steamItems)
    {
        await _steamItemsCollection.InsertManyAsync(steamItems.ConvertAll(i => i.ToSteamItemMongoDb()));
    }

    public async Task<IEnumerable<string>> GetSteamItemsIds()
    {
        return await _steamItemsCollection.AsQueryable().Select(doc => doc.Id!).ToListAsync();
    }
}
