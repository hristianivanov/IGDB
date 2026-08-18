namespace GameVault.Contracts.Games;

public interface ITwitchTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
}
