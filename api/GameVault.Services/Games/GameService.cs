using GameVault.Contracts.Games;
using GameVault.Services.Models.Games;

namespace GameVault.Services.Games;

public class GameService(IIgdbClient igdbClient)
{
    public Task<IReadOnlyList<GameSummaryDto>> SearchGamesAsync(GameSearchRequestDto request, CancellationToken cancellationToken)
        => igdbClient.SearchGamesAsync(request, cancellationToken);

    public Task<GameDetailsDto?> GetGameByIdAsync(long igdbGameId, CancellationToken cancellationToken)
        => igdbClient.GetGameByIdAsync(igdbGameId, cancellationToken);

    public Task<IReadOnlyList<GameSummaryDto>> GetPopularGamesAsync(int limit, CancellationToken cancellationToken)
        => igdbClient.GetPopularGamesAsync(limit, cancellationToken);
}
