using GameVault.Data.Models.Library;

namespace GameVault.Contracts.Library;

public interface ISavedGameRepository
{
    Task<IReadOnlyList<SavedGame>> GetAllAsync(CancellationToken cancellationToken);

    Task<SavedGame?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<SavedGame?> GetByExternalGameIdAsync(long externalGameId, CancellationToken cancellationToken);

    Task<SavedGame> AddAsync(SavedGame savedGame, CancellationToken cancellationToken);

    Task UpdateAsync(SavedGame savedGame, CancellationToken cancellationToken);

    Task DeleteAsync(SavedGame savedGame, CancellationToken cancellationToken);
}
