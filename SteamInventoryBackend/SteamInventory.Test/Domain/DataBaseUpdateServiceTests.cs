using Microsoft.Extensions.Logging;
using Moq;
using SteamInventory.BackgroundService;
using SteamInventory.Interfaces;
using SteamInventory.Models;
using SteamInventory.Repository.SteamItems;
using SteamInventory.Service;

namespace SteamInventory.Test.Domain;

public class DataBaseUpdateServiceTests
{
    private readonly Mock<ISteamItemsRepository> _mockSteamItemsRepository;
    private readonly Mock<ISteamWebApi> _mockSteamWebApi;
    private readonly Mock<ILogger<DataBaseUpdateService>> _mockLogger;
    private readonly DataBaseUpdateService _dataBaseUpdateService;

    public DataBaseUpdateServiceTests()
    {
        _mockSteamItemsRepository = new Mock<ISteamItemsRepository>();
        _mockSteamWebApi = new Mock<ISteamWebApi>();
        _mockLogger = new Mock<ILogger<DataBaseUpdateService>>();
        _dataBaseUpdateService = new DataBaseUpdateService(_mockSteamItemsRepository.Object, _mockLogger.Object, _mockSteamWebApi.Object);
    }

    [Fact]
    public async Task StartAsync_SchedulesDailyUpdate()
    {
        // Arrange
        var steamItemsRepositoryMock = new Mock<ISteamItemsRepository>();
        var loggerMock = new Mock<ILogger<DataBaseUpdateService>>();
        var steamWebApiMock = new Mock<ISteamWebApi>();

        var service = new DataBaseUpdateService(steamItemsRepositoryMock.Object, loggerMock.Object, steamWebApiMock.Object);

        // Act
        await service.StartAsync(CancellationToken.None);

        // Assert
        // Verify that the timer is created and scheduled for the next day
        Assert.NotNull(service._timer);
    }

    [Fact]
    public async Task DailyUpdateDataBaseHistory_Should_LogInformationAndUpdateDatabase()
    {
        // Arrange
        var existingItemIds = new List<string> { "1", "2", "3" };
        var allSteamItems = new List<SteamDailyItemsResponse>
            {
                new SteamDailyItemsResponse { Id = "1" },
                new SteamDailyItemsResponse { Id = "2" },
                new SteamDailyItemsResponse { Id = "3" }
            };

        _mockSteamItemsRepository.Setup(x => x.GetSteamItemsIds()).ReturnsAsync(existingItemIds);
        _mockSteamWebApi.Setup(x => x.GetAllItemsSteam()).ReturnsAsync(allSteamItems);

        // Act
        await _dataBaseUpdateService.DailyUpdateDataBaseHistory();
         
        // Assert
        _mockSteamItemsRepository.Verify(x => x.AddHistoryToExistingItem("1", It.IsAny<SteamItemHistory>()), Times.Once);
        _mockSteamItemsRepository.Verify(x => x.AddHistoryToExistingItem("2", It.IsAny<SteamItemHistory>()), Times.Once);
        _mockSteamItemsRepository.Verify(x => x.AddHistoryToExistingItem("3", It.IsAny<SteamItemHistory>()), Times.Once);
    }

}