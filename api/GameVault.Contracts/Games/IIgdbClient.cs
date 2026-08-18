using GameVault.Services.Models.Games;

namespace GameVault.Contracts.Games;

public interface IIgdbClient
{
    Task<IReadOnlyList<GameSummaryDto>> SearchGamesAsync(GameSearchRequestDto request, CancellationToken cancellationToken);

    Task<GameDetailsDto?> GetGameByIdAsync(long igdbGameId, CancellationToken cancellationToken);

    Task<IReadOnlyList<GameSummaryDto>> GetPopularGamesAsync(int limit, CancellationToken cancellationToken);
}
