namespace GameVault.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using GameVault.Services.Library;
using GameVault.Services.Models;
using GameVault.Services.Models.Library;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("library")]
public class LibraryController(LibraryService libraryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<SavedGameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await libraryService.GetAllAsync(cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(SavedGameDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CreateSavedGameRequestDto request, CancellationToken cancellationToken)
    {
        var savedGame = await libraryService.AddAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = savedGame.Id }, savedGame);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SavedGameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSavedGameRequestDto request, CancellationToken cancellationToken)
        => Ok(await libraryService.UpdateStatusAsync(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await libraryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
