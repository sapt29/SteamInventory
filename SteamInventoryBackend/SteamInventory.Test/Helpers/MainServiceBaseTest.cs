using Microsoft.Extensions.Logging;
using SteamInventory.Interfaces;
using SteamInventory.Service;

namespace SteamInventory.Test.Helpers;

public class MainServiceBaseTest
{
    private readonly IMainService _mainService;
    private readonly Mock<ILogger<MainService>> _logger = new();
    private readonly Mock<ISteamItemsRepository> _steamItemsRepository = new();
    private readonly Mock<ISteamWebApi> _steamWebApi = new();

    public MainServiceBaseTest()
    {
        _mainService = new MainService(_steamItemsRepository.Object, _steamWebApi.Object, _logger.Object);
    }

    [Fact]
    public Task Initialize_ShouldNotThrow()
    {
        // Arrange
        // Act
        //await _mainService();
        // Assert 
        _mainService.StartupInfo.Status.Should().Be(ServiceStatus.Initialized);
        return Task.CompletedTask;
    }
}
