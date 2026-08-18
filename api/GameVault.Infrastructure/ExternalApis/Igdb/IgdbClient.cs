using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using GameVault.Common.Exceptions;
using GameVault.Contracts.Games;
using GameVault.Infrastructure.ExternalApis.Igdb.Models;
using GameVault.Services.Models.Games;
using Microsoft.Extensions.Options;

namespace GameVault.Infrastructure.ExternalApis.Igdb;

public sealed class IgdbClient : IIgdbClient
{
    private const string SummaryFields =
        "fields name,cover.url,first_release_date,rating,genres.name;";

    private const string DetailsFields =
        "fields name,summary,cover.url,first_release_date,rating,rating_count," +
        "genres.name,platforms.name,screenshots.url,involved_companies.company.name;";

    private readonly HttpClient _http;
    private readonly ITwitchTokenProvider _tokenProvider;
    private readonly IgdbOptions _options;

    public IgdbClient(IHttpClientFactory httpClientFactory, ITwitchTokenProvider tokenProvider, IOptions<IgdbOptions> options)
    {
        _http = httpClientFactory.CreateClient("Igdb");
        _tokenProvider = tokenProvider;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<GameSummaryDto>> SearchGamesAsync(GameSearchRequestDto request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 50);
        var offset = Math.Max(request.Offset, 0);

        var body = new StringBuilder(SummaryFields);
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            body.Append($" search \"{EscapeApicalypseString(request.Query)}\";");
        }
        body.Append($" limit {limit}; offset {offset};");

        var games = await PostAsync(body.ToString(), cancellationToken);
        return games.Select(MapToSummary).ToList();
    }

    public async Task<GameDetailsDto?> GetGameByIdAsync(long igdbGameId, CancellationToken cancellationToken)
    {
        var body = $"{DetailsFields} where id = {igdbGameId};";
        var games = await PostAsync(body, cancellationToken);
        var game = games.FirstOrDefault();
        return game is null ? null : MapToDetails(game);
    }

    public async Task<IReadOnlyList<GameSummaryDto>> GetPopularGamesAsync(int limit, CancellationToken cancellationToken)
    {
        var clampedLimit = Math.Clamp(limit, 1, 50);
        var body = $"{SummaryFields} where rating_count > 50; sort rating_count desc; limit {clampedLimit};";

        var games = await PostAsync(body, cancellationToken);
        return games.Select(MapToSummary).ToList();
    }

    private async Task<List<IgdbGameRaw>> PostAsync(string apicalypseBody, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "games")
            {
                Content = new StringContent(apicalypseBody, Encoding.UTF8, "text/plain"),
            };
            requestMessage.Headers.Add("Client-ID", _options.ClientId);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _http.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            var games = await response.Content.ReadFromJsonAsync<List<IgdbGameRaw>>(cancellationToken: cancellationToken);
            return games ?? [];
        }
        catch (HttpRequestException ex)
        {
            throw new IgdbUnavailableException("Failed to reach the IGDB API.", ex);
        }
    }

    private static GameSummaryDto MapToSummary(IgdbGameRaw raw) => new()
    {
        Id = raw.Id,
        Name = raw.Name,
        CoverUrl = ToCoverUrl(raw.Cover?.Url),
        FirstReleaseDate = ToDateTime(raw.FirstReleaseDateUnix),
        Rating = raw.Rating,
        Genres = raw.Genres.Select(g => g.Name).ToList(),
    };

    private static GameDetailsDto MapToDetails(IgdbGameRaw raw) => new()
    {
        Id = raw.Id,
        Name = raw.Name,
        Summary = raw.Summary,
        CoverUrl = ToCoverUrl(raw.Cover?.Url),
        FirstReleaseDate = ToDateTime(raw.FirstReleaseDateUnix),
        Rating = raw.Rating,
        RatingCount = raw.RatingCount,
        Genres = raw.Genres.Select(g => g.Name).ToList(),
        Platforms = raw.Platforms.Select(p => p.Name).ToList(),
        Screenshots = raw.Screenshots.Select(s => ToCoverUrl(s.Url, "t_screenshot_big")).Where(u => u is not null).Select(u => u!).ToList(),
        InvolvedCompanies = raw.InvolvedCompanies.Where(c => c.Company is not null).Select(c => c.Company!.Name).Distinct().ToList(),
    };

    private static string? ToCoverUrl(string? rawUrl, string size = "t_cover_big")
    {
        if (string.IsNullOrWhiteSpace(rawUrl))
            return null;

        var normalized = rawUrl.StartsWith("//", StringComparison.Ordinal) ? $"https:{rawUrl}" : rawUrl;
        return normalized.Replace("t_thumb", size, StringComparison.Ordinal);
    }

    private static DateTime? ToDateTime(long? unixSeconds)
        => unixSeconds.HasValue ? DateTimeOffset.FromUnixTimeSeconds(unixSeconds.Value).UtcDateTime : null;

    private static string EscapeApicalypseString(string value)
        => value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);
}
