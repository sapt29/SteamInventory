using AutoFixture;
using Microsoft.Extensions.Logging;
using SteamInventory.Interfaces;
using SteamInventory.Models;
using SteamInventory.Service;

namespace SteamInventory.Test.Domain;

public class MainServiceTests
{
    private Fixture _fixture = new();
    private readonly Mock<ISteamItemsRepository> _mockSteamItemsRepository = new();
    private readonly Mock<ISteamWebApi> _mockSteamWebApi = new();
    private readonly Mock<ILogger<MainService>> _mockLogger = new();

    [Fact]
    public async Task GetUserInventoryByIdAsync_Success()
    {
        // Arrange
        var userId = "123456789";
        var userProfile = new UserInfoSteamResponse { AvatarFull = "avatar.jpg", PersonaName = "User123" };
        var steamItemsDb = _fixture.CreateMany<SteamItem>(3).ToList();
        var userInventoryItems = new List<UserInventoryItemsSteamResponse>
        {
            new() {Id = steamItemsDb[0].Id, Markethashname = steamItemsDb[0].ItemHashName, Image = steamItemsDb[0].Image, Count = 5, Pricelatest = 3.55 },
            new() {Id = steamItemsDb[1].Id, Markethashname = steamItemsDb[1].ItemHashName, Image = steamItemsDb[1].Image, Count = 1, Pricelatest = 0.3 },
            new() {Id = steamItemsDb[2].Id, Markethashname = steamItemsDb[2].ItemHashName, Image = steamItemsDb[2].Image, Count = 2, Pricelatest = 1.22 }
        };
        var userInventoryHistorySteamResponse = _fixture.CreateMany<UserInventoryHistorySteamResponse>(3).ToList();

        _mockSteamItemsRepository.Setup(repo => repo.GetSteamItemsByIdsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(steamItemsDb);

        _mockSteamWebApi.Setup(api => api.GetUserInfoSteam(userId)).ReturnsAsync(userProfile);
        _mockSteamWebApi.Setup(api => api.GetUserInventoryItemsSteamByUserId(userId)).ReturnsAsync(userInventoryItems);
        _mockSteamWebApi.Setup(api => api.GetHistoryItemSteam(It.IsAny<string>()))
            .ReturnsAsync(userInventoryHistorySteamResponse);

        var mainService = new MainService(_mockSteamItemsRepository.Object, _mockSteamWebApi.Object, _mockLogger.Object);

        // Act
        var result = await mainService.GetUserInventoryByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userProfile.AvatarFull, result.UserProfilePicture);
        Assert.Equal(userProfile.PersonaName, result.UserName);
        Assert.NotNull(result.Items);
        Assert.Equal(userInventoryItems.Count, result.Items.Count);
    }

    [Fact]
    public async Task GetUserInventoryByIdAsync_WithNotExistingItems_Success()
    {
        // Arrange
        var userId = "123456789";
        var userProfile = new UserInfoSteamResponse { AvatarFull = "avatar.jpg", PersonaName = "User123" };

        var userInventoryItems = new List<UserInventoryItemsSteamResponse>
        {
            _fixture.Create<UserInventoryItemsSteamResponse>(),
            _fixture.Create<UserInventoryItemsSteamResponse>(),
            _fixture.Create<UserInventoryItemsSteamResponse>(),
        };

        var notExistingItems = new List<UserInventoryItemsSteamResponse>
        {
            _fixture.Create<UserInventoryItemsSteamResponse>(),
            _fixture.Create<UserInventoryItemsSteamResponse>(),
        };
        var UserInventoryHistorySteamResponses = _fixture.CreateMany<UserInventoryHistorySteamResponse>(20).ToList();
        var historyItems = new List<SteamItemHistory>();
        foreach (var history in UserInventoryHistorySteamResponses)
        {
            historyItems.Add(new SteamItemHistory()
            {
                Average = history.Avg,
                Date = history.Createdat,
                ItemsSold = history.Sold,
                LastPrice = history.Price
            });
        }
        
        _mockSteamWebApi.Setup(api => api.GetUserInfoSteam(userId)).ReturnsAsync(userProfile);
        _mockSteamWebApi.Setup(api => api.GetUserInventoryItemsSteamByUserId(userId)).ReturnsAsync(userInventoryItems);

        _mockSteamWebApi.Setup(api => api.GetHistoryItemSteam(userInventoryItems[0].Markethashname!))
            .ReturnsAsync(UserInventoryHistorySteamResponses.GetRange(0, 5));
        _mockSteamWebApi.Setup(api => api.GetHistoryItemSteam(userInventoryItems[1].Markethashname!))
            .ReturnsAsync(UserInventoryHistorySteamResponses.GetRange(5, 5));

        _mockSteamWebApi.Setup(api => api.GetHistoryItemSteam(notExistingItems[0].Markethashname!))
            .ReturnsAsync(UserInventoryHistorySteamResponses.GetRange(9, 5));
        _mockSteamWebApi.Setup(api => api.GetHistoryItemSteam(notExistingItems[1].Markethashname!))
            .ReturnsAsync(UserInventoryHistorySteamResponses.GetRange(14, 5));

        var itemsToAdd = new List<SteamItem>()
        {
            new() {Id = userInventoryItems[0].Id, ItemHashName = userInventoryItems[0].Markethashname, Image = userInventoryItems[0].Image, History = historyItems.GetRange(0,5) },
            new() {Id = userInventoryItems[1].Id, ItemHashName = userInventoryItems[1].Markethashname, Image = userInventoryItems[1].Image, History = historyItems.GetRange(5,5) },
            new() {Id = userInventoryItems[2].Id, ItemHashName = userInventoryItems[2].Markethashname, Image = userInventoryItems[2].Image, History = [new() { Average = 9.35, Date = DateTime.UtcNow, ItemsSold = 110, LastPrice = 9.36}, new() { Average = 9.35, Date = DateTime.UtcNow.AddDays(-1), ItemsSold = 200, LastPrice = 9.40}] },
            new() {Id = notExistingItems[0].Id, ItemHashName = notExistingItems[0].Markethashname, Image = notExistingItems[0].Image, History = historyItems.GetRange(9,5) },
            new() {Id = notExistingItems[1].Id, ItemHashName = notExistingItems[1].Markethashname, Image = notExistingItems[1].Image, History = historyItems.GetRange(14,5) }
        };

        _mockSteamItemsRepository.SetupSequence(repo => repo.GetSteamItemsByIdsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<SteamItem>())
            .ReturnsAsync(itemsToAdd);

        var mainService = new MainService(_mockSteamItemsRepository.Object, _mockSteamWebApi.Object, _mockLogger.Object);

        // Act
        var result = await mainService.GetUserInventoryByIdAsync(userId);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(userProfile.AvatarFull, result.UserProfilePicture);
        Assert.Equal(userProfile.PersonaName, result.UserName);
        Assert.Equal(userInventoryItems[0].Markethashname, result.Items[0].ItemName);
        Assert.NotNull(result.Items);
        Assert.NotNull(result.Items[1].History); 
    }

    [Fact]
    public async Task GetUserInventoryByIdAsync_ExceptionThrown()
    {
        // Arrange
        var userId = "123456789";
        _mockSteamWebApi.Setup(api => api.GetUserInfoSteam(userId)).ThrowsAsync(new Exception("Simulated error"));

        var mainService = new MainService(_mockSteamItemsRepository.Object, _mockSteamWebApi.Object, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mainService.GetUserInventoryByIdAsync(userId));
    }

    [Fact]
    public async Task GetUserInventoryItemsSteamByUserI_ExceptionThrown()
    {
        // Arrange
        var userId = "123456789";
        _mockSteamWebApi.Setup(api => api.GetUserInventoryItemsSteamByUserId(userId)).ThrowsAsync(new Exception("Simulated error"));

        var mainService = new MainService(_mockSteamItemsRepository.Object, _mockSteamWebApi.Object, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mainService.GetUserInventoryByIdAsync(userId));
    }

    [Fact]
    public async Task GetHistoryItemSteam_ExceptionThrown()
    {
        // Arrange
        var userId = "123456789";
        var userProfile = new UserInfoSteamResponse { AvatarFull = "avatar.jpg", PersonaName = "User123" };
        _mockSteamItemsRepository.Setup(repo => repo.GetSteamItemsByIdsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<SteamItem>());

        _mockSteamWebApi.Setup(api => api.GetUserInventoryItemsSteamByUserId(userId)).ReturnsAsync(_fixture.CreateMany<UserInventoryItemsSteamResponse>(5));
        _mockSteamWebApi.Setup(api => api.GetHistoryItemSteam(It.IsAny<string>())).ThrowsAsync(new Exception("Simulated error"));

        var mainService = new MainService(_mockSteamItemsRepository.Object, _mockSteamWebApi.Object, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mainService.GetUserInventoryByIdAsync(userId));
    }
}