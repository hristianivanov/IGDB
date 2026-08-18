using GameVault.Contracts.Library;
using GameVault.Data.Models.Library;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data.Repositories;

public class SavedGameRepository(ApplicationDbContext dbContext) : ISavedGameRepository
{
    public async Task<IReadOnlyList<SavedGame>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.SavedGames
            .AsNoTracking()
            .OrderByDescending(g => g.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<SavedGame?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await dbContext.SavedGames
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public async Task<SavedGame?> GetByIgdbGameIdAsync(long igdbGameId, CancellationToken cancellationToken)
        => await dbContext.SavedGames
            .FirstOrDefaultAsync(g => g.IgdbGameId == igdbGameId, cancellationToken);

    public async Task<SavedGame> AddAsync(SavedGame savedGame, CancellationToken cancellationToken)
    {
        dbContext.SavedGames.Add(savedGame);
        await dbContext.SaveChangesAsync(cancellationToken);
        return savedGame;
    }

    public async Task UpdateAsync(SavedGame savedGame, CancellationToken cancellationToken)
    {
        dbContext.SavedGames.Update(savedGame);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SavedGame savedGame, CancellationToken cancellationToken)
    {
        dbContext.SavedGames.Remove(savedGame);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
