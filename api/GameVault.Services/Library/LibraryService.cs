using GameVault.Common.Constants;
using GameVault.Common.Exceptions;
using GameVault.Contracts.Library;
using GameVault.Data.Models.Library;
using GameVault.Services.Models.Library;
using Mapster;

namespace GameVault.Services.Library;

public class LibraryService(ISavedGameRepository savedGameRepository)
{
    public async Task<IReadOnlyList<SavedGameDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var savedGames = await savedGameRepository.GetAllAsync(cancellationToken);
        return savedGames.Select(g => g.Adapt<SavedGameDto>()).ToList();
    }

    public async Task<SavedGameDto> AddAsync(CreateSavedGameRequestDto request, CancellationToken cancellationToken)
    {
        var status = ParseStatus(request.Status);

        var existing = await savedGameRepository.GetByIgdbGameIdAsync(request.IgdbGameId, cancellationToken);
        if (existing is not null)
            throw new ValidationException($"Game '{request.IgdbGameId}' is already in your library.");

        var now = DateTime.UtcNow;
        var savedGame = new SavedGame
        {
            Id = Guid.NewGuid(),
            IgdbGameId = request.IgdbGameId,
            Name = request.Name,
            CoverUrl = request.CoverUrl,
            Status = status,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        var created = await savedGameRepository.AddAsync(savedGame, cancellationToken);
        return created.Adapt<SavedGameDto>();
    }

    public async Task<SavedGameDto> UpdateStatusAsync(Guid id, UpdateSavedGameRequestDto request, CancellationToken cancellationToken)
    {
        var status = ParseStatus(request.Status);

        var savedGame = await savedGameRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Saved game '{id}' was not found.");

        savedGame.Status = status;
        savedGame.UpdatedAtUtc = DateTime.UtcNow;

        await savedGameRepository.UpdateAsync(savedGame, cancellationToken);
        return savedGame.Adapt<SavedGameDto>();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var savedGame = await savedGameRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Saved game '{id}' was not found.");

        await savedGameRepository.DeleteAsync(savedGame, cancellationToken);
    }

    private static GameListStatus ParseStatus(string status)
    {
        if (!AllowedGameListStatuses.Values.Contains(status) ||
            !Enum.TryParse<GameListStatus>(status, ignoreCase: true, out var parsed))
        {
            throw new ValidationException(
                $"Status must be one of: {string.Join(", ", AllowedGameListStatuses.Values)}.");
        }

        return parsed;
    }
}
