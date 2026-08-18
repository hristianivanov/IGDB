using System.Net.Http.Json;
using GameVault.Common.Exceptions;
using GameVault.Contracts.Games;
using GameVault.Infrastructure.ExternalApis.Igdb.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace GameVault.Infrastructure.ExternalApis.Igdb;

public sealed class TwitchTokenProvider : ITwitchTokenProvider
{
    private const string CacheKey = "twitch:access-token";
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly IgdbOptions _options;

    public TwitchTokenProvider(IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<IgdbOptions> options)
    {
        _http = httpClientFactory.CreateClient("TwitchAuth");
        _cache = cache;
        _options = options.Value;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue<string>(CacheKey, out var cachedToken) && cachedToken is not null)
            return cachedToken;

        await RefreshLock.WaitAsync(cancellationToken);
        try
        {
            if (_cache.TryGetValue<string>(CacheKey, out cachedToken) && cachedToken is not null)
                return cachedToken;

            if (string.IsNullOrWhiteSpace(_options.ClientId) || string.IsNullOrWhiteSpace(_options.ClientSecret))
            {
                throw new IgdbUnavailableException(
                    "Twitch/IGDB credentials are not configured. Set Igdb:ClientId and Igdb:ClientSecret.");
            }

            var url = $"https://id.twitch.tv/oauth2/token?client_id={_options.ClientId}" +
                       $"&client_secret={_options.ClientSecret}&grant_type=client_credentials";

            using var response = await _http.PostAsync(url, content: null, cancellationToken);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<TwitchTokenResponse>(cancellationToken)
                ?? throw new IgdbUnavailableException("Twitch returned an empty token response.");

            var ttl = TimeSpan.FromSeconds(Math.Max(token.ExpiresInSeconds - 60, 60));
            _cache.Set(CacheKey, token.AccessToken, ttl);

            return token.AccessToken;
        }
        catch (HttpRequestException ex)
        {
            throw new IgdbUnavailableException("Failed to obtain a Twitch access token for IGDB.", ex);
        }
        finally
        {
            RefreshLock.Release();
        }
    }
}
