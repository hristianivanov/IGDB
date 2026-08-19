using GameVault.Services.Models.Games;

namespace GameVault.Contracts.Games;

public interface IGameCatalogClient
{
    Task<IReadOnlyList<GameSummaryDto>> SearchGamesAsync(GameSearchRequestDto request, CancellationToken cancellationToken);

    Task<GameDetailsDto?> GetGameByIdAsync(long externalGameId, CancellationToken cancellationToken);

    Task<IReadOnlyList<GameSummaryDto>> GetPopularGamesAsync(int limit, CancellationToken cancellationToken);
}
