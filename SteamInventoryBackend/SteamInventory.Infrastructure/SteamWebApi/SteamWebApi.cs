using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SteamInventory.Interfaces;
using SteamInventory.Service;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamInventory.Infrastructure.SteamWebApi;

public class SteamWebApi : ISteamWebApi
{
    private readonly HttpClient _client;  
    private static JsonSerializerOptions jsonSerializerOptions;
    private readonly SteamWebApiConfiguration _steamConfiguration;
    private readonly ILogger<SteamWebApi> _logger;

    public SteamWebApi(IOptions<SteamWebApiConfiguration> steamConfiguration, ILogger<SteamWebApi> logger, HttpClient? client = null)
    {
        _steamConfiguration = steamConfiguration.Value;
        _logger = logger;

        _client = client ?? new HttpClient
        {
            BaseAddress = new Uri(_steamConfiguration.BaseUrl!),
        };

        jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    // Get all CS2 steam items /steam/api/items
    public async Task<IEnumerable<SteamDailyItemsResponse>> GetAllItemsSteam()
    {
        var response = await _client.GetAsync($"{Consts.GetAllItemsSteamEndpoint}?key={_steamConfiguration.ApiKey}");

        if (!response.IsSuccessStatusCode)
        {
            var steamError = JsonSerializer.Deserialize<SteamErrorResponse>(response.Content.ReadAsStringAsync().Result, jsonSerializerOptions);
            _logger.LogError("GetUserInfoSteam: There was an error getting all the CS2 items: {}", steamError);
        }

        var getResponse = JsonSerializer.Deserialize<SteamDailyItemsResponse[]>(response.Content.ReadAsStringAsync().Result, jsonSerializerOptions);
        return getResponse!;
    }

    // Get Name and profilePic /steam/api/profile
    public async Task<UserInfoSteamResponse> GetUserInfoSteam(string userId)
    {
        var response = await _client.GetAsync($"{Consts.GetUserInfoSteamEndpoint}?key={_steamConfiguration.ApiKey}&id={userId}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("GetUserInfoSteam: There was an error getting the UserInfoSteam from the userID: {}", userId);
            var steamError = JsonSerializer.Deserialize<SteamErrorResponse>(response.Content.ReadAsStringAsync().Result, jsonSerializerOptions);
            throw new HttpRequestException(steamError?.Error);
        }

        var getResponse = JsonSerializer.Deserialize<UserInfoSteamResponse>(response.Content.ReadAsStringAsync().Result, jsonSerializerOptions);
        return getResponse!;
    }

    // Get User Steam Inventory /steam/api/inventory
    public async Task<IEnumerable<UserInventoryItemsSteamResponse>> GetUserInventoryItemsSteamByUserId(string userId)
    {
        var response = await _client.GetAsync($"{Consts.GetUserInventoryItemsSteamEndpoint}?key={_steamConfiguration.ApiKey}&steam_id={userId}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("GetUserInventoryItemsSteamByUserId: There was an error getting the user inventory from the userID: {}", userId);
            var steamError = JsonSerializer.Deserialize<SteamErrorResponse>(response.Content.ReadAsStringAsync().Result, jsonSerializerOptions);
            throw new HttpRequestException(steamError?.Error);
        }

        var getResponse = JsonSerializer.Deserialize<UserInventoryItemsSteamResponse[]>(response.Content.ReadAsStringAsync().Result, jsonSerializerOptions);
        return DeleteDuplicatedInventoryItems(getResponse);
    }

    // Get history of an item /steam/api/history
    public async Task<IEnumerable<UserInventoryHistorySteamResponse>> GetHistoryItemSteam(string itemName)
    {
        var itemNameFormatted = itemName.Replace("&", "%26");
        var currentDate = DateTime.Now;
        var threeMonthsDate = currentDate.AddMonths(-3).AddDays(1 - currentDate.Day);
        var response = await _client.GetAsync($"{Consts.GetHistoryItemSteamEndpoint}?key={_steamConfiguration.ApiKey}&market_hash_name={itemNameFormatted}&interval=1&start_date={threeMonthsDate:yyyy-MM-dd}&end_date={currentDate:yyyy-MM-dd}");

        if (!response.IsSuccessStatusCode)
        {
            var steamError = JsonSerializer.Deserialize<SteamErrorResponse>(response.Content.ReadAsStringAsync().Result, jsonSerializerOptions);

            if (steamError?.Error == "Item Prices not exists")
            {
                _logger.LogWarning("GetHistoryItemSteam: Item Prices not exists for item: {}", itemName);
                return new List<UserInventoryHistorySteamResponse> { new() };
            }
            if (steamError?.Error == "Item not found")
            {
                _logger.LogWarning("GetHistoryItemSteam: Item {} was not found by Steam Web API", itemName);
                return new List<UserInventoryHistorySteamResponse> { new() };
            }
            _logger.LogError("GetHistoryItemSteam: Steam Web API error: {}", steamError?.Error);
            throw new HttpRequestException(steamError?.Error);
        }

        var getResponse = JsonSerializer.Deserialize<UserInventoryHistorySteamResponse[]>(response.Content.ReadAsStringAsync().Result, jsonSerializerOptions);
        return getResponse!;
    }

    private static IEnumerable<UserInventoryItemsSteamResponse> DeleteDuplicatedInventoryItems(UserInventoryItemsSteamResponse[]? getResponse)
    {
        return getResponse!.GroupBy(item => item.Id)
                                   .Select(group =>
                                   {
                                       var firstItem = group.First();
                                       firstItem.Count = group.Sum(item => item.Count);
                                       return firstItem;
                                   })
                                   .ToList();
    }
}
