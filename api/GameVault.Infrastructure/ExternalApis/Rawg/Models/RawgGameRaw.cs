using System.Text.Json.Serialization;

namespace GameVault.Infrastructure.ExternalApis.Rawg.Models;

public class RawgGameRaw
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description_raw")]
    public string? DescriptionRaw { get; set; }

    [JsonPropertyName("released")]
    public string? Released { get; set; }

    [JsonPropertyName("background_image")]
    public string? BackgroundImage { get; set; }

    [JsonPropertyName("rating")]
    public double? Rating { get; set; }

    [JsonPropertyName("ratings_count")]
    public int? RatingsCount { get; set; }

    [JsonPropertyName("genres")]
    public List<RawgNamedEntityRaw> Genres { get; set; } = [];

    [JsonPropertyName("platforms")]
    public List<RawgPlatformEntryRaw> Platforms { get; set; } = [];

    [JsonPropertyName("short_screenshots")]
    public List<RawgScreenshotRaw> ShortScreenshots { get; set; } = [];

    [JsonPropertyName("developers")]
    public List<RawgNamedEntityRaw> Developers { get; set; } = [];

    [JsonPropertyName("publishers")]
    public List<RawgNamedEntityRaw> Publishers { get; set; } = [];
}

public class RawgNamedEntityRaw
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class RawgPlatformEntryRaw
{
    [JsonPropertyName("platform")]
    public RawgNamedEntityRaw? Platform { get; set; }
}

public class RawgScreenshotRaw
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }
}

public class RawgSearchResponseRaw
{
    [JsonPropertyName("results")]
    public List<RawgGameRaw> Results { get; set; } = [];
}
