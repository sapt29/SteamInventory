using Microsoft.AspNetCore.Mvc;
using SteamInventory.Api.Helpers;
using SteamInventory.Api.Models;
using SteamInventory.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;
namespace SteamInventory.Api.Controllers;

[ApiController]
[Route("/inventory")]
public class SteamInventoryController : ControllerBase
{
    private readonly IMainService _mainService;
    private readonly ILogger<SteamInventoryController> _logger;

    public SteamInventoryController(IMainService mainService, ILogger<SteamInventoryController> logger)
    {
        _mainService = mainService;
        _logger = logger;
    }

    [HttpGet]
    [SwaggerOperation (Description = "Get from user steam profile, all the user Counter Strike 2 items with price history and percentage change, using steam ID", Summary = "Get all the user Counter Strike 2 items")]
    [SwaggerResponse(statusCode:200, type: typeof(GetUserInventoryApiResponse), description: "Items price and history obtained")]
    [SwaggerResponse(statusCode:400, type: typeof(string), description: "Some problem with user inventory or user does not exist")]
    [SwaggerResponse(statusCode:500, type: typeof(string), description: "Internal server error or another not supported error")]
    public async Task<IActionResult> GetUserInventoryById([FromQuery, SwaggerParameter("User Steam ID", Required = true)] string userSteamId)
    {
        if (userSteamId.Length != 17)
        {
            _logger.LogError("Steam userId doesn't meet the requirements");
            
            return BadRequest("Steam userId must be a 17 digit number");
        }
        if (!Regex.IsMatch(userSteamId, @"^\d+$"))
        {
            _logger.LogError("Steam userId doesn't meet the requirements");
            return BadRequest("Steam userId can not contain letters");
        }
        try
        {
            var result = await _mainService.GetUserInventoryByIdAsync(userSteamId);
            return Ok(TypeMapper.ToApi(result));
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Steam Web API error {}", e.Message);
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Request could not be completed, error {}", e.Message);
            return Problem("Internal server error");
        }
    }
}
