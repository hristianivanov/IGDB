using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using GameVault.Common.Exceptions;
using GameVault.Contracts.Games;
using GameVault.Infrastructure.ExternalApis.Rawg.Models;
using GameVault.Services.Models.Games;
using Microsoft.Extensions.Options;

namespace GameVault.Infrastructure.ExternalApis.Rawg;

public sealed class RawgClient : IGameCatalogClient
{
    private readonly HttpClient _http;
    private readonly RawgOptions _options;

    public RawgClient(IHttpClientFactory httpClientFactory, IOptions<RawgOptions> options)
    {
        _http = httpClientFactory.CreateClient("Rawg");
        _options = options.Value;
    }

    public async Task<IReadOnlyList<GameSummaryDto>> SearchGamesAsync(GameSearchRequestDto request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 40);
        var page = Math.Max((request.Offset / limit) + 1, 1);

        var query = $"games?key={ApiKey()}&page_size={limit}&page={page}";
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            query += $"&search={Uri.EscapeDataString(request.Query)}";
        }

        var response = await GetAsync<RawgSearchResponseRaw>(query, cancellationToken);
        return response?.Results.Select(MapToSummary).ToList() ?? [];
    }

    public async Task<GameDetailsDto?> GetGameByIdAsync(long externalGameId, CancellationToken cancellationToken)
    {
        try
        {
            using var httpResponse = await _http.GetAsync($"games/{externalGameId}?key={ApiKey()}", cancellationToken);

            if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                return null;

            httpResponse.EnsureSuccessStatusCode();

            var raw = await httpResponse.Content.ReadFromJsonAsync<RawgGameRaw>(cancellationToken: cancellationToken);
            return raw is null ? null : MapToDetails(raw);
        }
        catch (HttpRequestException ex)
        {
            throw new GameCatalogUnavailableException("Failed to reach the RAWG API.", ex);
        }
    }

    public async Task<IReadOnlyList<GameSummaryDto>> GetPopularGamesAsync(int limit, CancellationToken cancellationToken)
    {
        var clampedLimit = Math.Clamp(limit, 1, 40);
        var query = $"games?key={ApiKey()}&ordering=-added&page_size={clampedLimit}";

        var response = await GetAsync<RawgSearchResponseRaw>(query, cancellationToken);
        return response?.Results.Select(MapToSummary).ToList() ?? [];
    }

    private async Task<T?> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _http.GetAsync(relativeUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new GameCatalogUnavailableException("Failed to reach the RAWG API.", ex);
        }
    }

    private string ApiKey()
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new GameCatalogUnavailableException(
                "RAWG credentials are not configured. Set Rawg:ApiKey.");
        }

        return _options.ApiKey;
    }

    private static GameSummaryDto MapToSummary(RawgGameRaw raw) => new()
    {
        Id = raw.Id,
        Name = raw.Name,
        CoverUrl = raw.BackgroundImage,
        FirstReleaseDate = ParseReleaseDate(raw.Released),
        Rating = ToHundredScale(raw.Rating),
        Genres = raw.Genres.Select(g => g.Name).ToList(),
    };

    private static GameDetailsDto MapToDetails(RawgGameRaw raw) => new()
    {
        Id = raw.Id,
        Name = raw.Name,
        Summary = raw.DescriptionRaw,
        CoverUrl = raw.BackgroundImage,
        FirstReleaseDate = ParseReleaseDate(raw.Released),
        Rating = ToHundredScale(raw.Rating),
        RatingCount = raw.RatingsCount,
        Genres = raw.Genres.Select(g => g.Name).ToList(),
        Platforms = raw.Platforms.Where(p => p.Platform is not null).Select(p => p.Platform!.Name).ToList(),
        Screenshots = raw.ShortScreenshots
            .Where(s => s.Id > 0 && !string.IsNullOrWhiteSpace(s.Image))
            .Select(s => s.Image!)
            .ToList(),
        InvolvedCompanies = raw.Developers.Select(d => d.Name)
            .Concat(raw.Publishers.Select(p => p.Name))
            .Distinct()
            .ToList(),
    };

    private static double? ToHundredScale(double? ratingOutOfFive)
        => ratingOutOfFive.HasValue ? Math.Round(ratingOutOfFive.Value * 20, 0) : null;

    private static DateTime? ParseReleaseDate(string? released)
        => DateTime.TryParseExact(released, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null;
}
