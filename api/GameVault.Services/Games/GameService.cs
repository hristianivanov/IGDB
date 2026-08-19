using GameVault.Contracts.Games;
using GameVault.Services.Models.Games;

namespace GameVault.Services.Games;

public class GameService(IGameCatalogClient gameCatalogClient)
{
    public Task<IReadOnlyList<GameSummaryDto>> SearchGamesAsync(GameSearchRequestDto request, CancellationToken cancellationToken)
        => gameCatalogClient.SearchGamesAsync(request, cancellationToken);

    public Task<GameDetailsDto?> GetGameByIdAsync(long externalGameId, CancellationToken cancellationToken)
        => gameCatalogClient.GetGameByIdAsync(externalGameId, cancellationToken);

    public Task<IReadOnlyList<GameSummaryDto>> GetPopularGamesAsync(int limit, CancellationToken cancellationToken)
        => gameCatalogClient.GetPopularGamesAsync(limit, cancellationToken);
}
