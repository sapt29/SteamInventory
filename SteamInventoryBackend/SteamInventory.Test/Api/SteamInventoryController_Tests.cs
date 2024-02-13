using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SteamInventory.Api.Controllers;
using SteamInventory.Api.Models;
using SteamInventory.Interfaces;
using SteamInventory.Service;
using System.Net;

namespace SteamInventory.Test.Api;

public class SteamInventoryController_Tests
{
    private Fixture _fixture = new();
    private readonly Mock<IMainService> _mockMainService = new();
    private readonly Mock<ILogger<SteamInventoryController>> _logger = new();
    [Fact]
    public async Task GetUserInventoryById_ValidUserId_ReturnsOk()
    {
        // Arrange
        var response = _fixture.Create<GetUserInventoryResponse>();

        var userSteamId = "12345678901234567";
        _mockMainService.Setup(service => service.GetUserInventoryByIdAsync(userSteamId))
            .ReturnsAsync(response);

        var controller = new SteamInventoryController(_mockMainService.Object, _logger.Object);

        // Act
        var result = await controller.GetUserInventoryById(userSteamId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<GetUserInventoryApiResponse>(result.Value);
    }

    [Fact]
    public async Task GetUserInventoryById_InvalidUserId_LengthNot17_ReturnsBadRequest()
    {
        // Arrange
        var invalidUserSteamId = "123";
        var controller = new SteamInventoryController(_mockMainService.Object, _logger.Object);

        // Act
        var result = await controller.GetUserInventoryById(invalidUserSteamId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Steam userId must be a 17 digit number", result.Value);
    }

    [Fact]
    public async Task GetUserInventoryById_InvalidUserId_ContainsLetters_ReturnsBadRequest()
    {
        // Arrange
        var invalidUserSteamId = "1234567890123456a";
        var controller = new SteamInventoryController(_mockMainService.Object, _logger.Object);

        // Act
        var result = await controller.GetUserInventoryById(invalidUserSteamId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Steam userId can not contain letters", result.Value);
    }

    [Fact]
    public async Task GetUserInventoryById_MainServiceThrowsHttpRequestException_ReturnsBadRequest()
    {
        // Arrange
        var userSteamId = "12345678901234567";
        _mockMainService.Setup(service => service.GetUserInventoryByIdAsync(userSteamId))
            .ThrowsAsync(new HttpRequestException("Mocked HTTP request error"));

        var controller = new SteamInventoryController(_mockMainService.Object, _logger.Object);

        // Act
        var result = await controller.GetUserInventoryById(userSteamId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetUserInventoryById_MainServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var userSteamId = "12345678901234567";
        _mockMainService.Setup(service => service.GetUserInventoryByIdAsync(userSteamId))
            .ThrowsAsync(new Exception("Mocked internal error"));

        var controller = new SteamInventoryController(_mockMainService.Object, _logger.Object);

        // Act
        var result = await controller.GetUserInventoryById(userSteamId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
    }
}