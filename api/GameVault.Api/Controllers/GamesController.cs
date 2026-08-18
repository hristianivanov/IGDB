namespace GameVault.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using GameVault.Services.Games;
using GameVault.Services.Models;
using GameVault.Services.Models.Games;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("games")]
public class GamesController(GameService gameService) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<GameSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Search(
        [FromQuery] string? query,
        [FromQuery] int limit,
        [FromQuery] int offset,
        CancellationToken cancellationToken)
    {
        var request = new GameSearchRequestDto
        {
            Query = query,
            Limit = limit == 0 ? 20 : limit,
            Offset = offset,
        };

        return Ok(await gameService.SearchGamesAsync(request, cancellationToken));
    }

    [HttpGet("popular")]
    [ProducesResponseType(typeof(List<GameSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Popular([FromQuery] int limit, CancellationToken cancellationToken)
        => Ok(await gameService.GetPopularGamesAsync(limit == 0 ? 20 : limit, cancellationToken));

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GameDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var game = await gameService.GetGameByIdAsync(id, cancellationToken);
        return game is null ? NotFound(new ErrorResponseDto { Error = $"Game '{id}' was not found." }) : Ok(game);
    }
}
