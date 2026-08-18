using System.Text.Json.Serialization;

namespace GameVault.Infrastructure.ExternalApis.Igdb.Models;

public class IgdbGameRaw
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("first_release_date")]
    public long? FirstReleaseDateUnix { get; set; }

    [JsonPropertyName("rating")]
    public double? Rating { get; set; }

    [JsonPropertyName("rating_count")]
    public int? RatingCount { get; set; }

    [JsonPropertyName("cover")]
    public IgdbCoverRaw? Cover { get; set; }

    [JsonPropertyName("genres")]
    public List<IgdbNamedEntityRaw> Genres { get; set; } = [];

    [JsonPropertyName("platforms")]
    public List<IgdbNamedEntityRaw> Platforms { get; set; } = [];

    [JsonPropertyName("screenshots")]
    public List<IgdbImageRaw> Screenshots { get; set; } = [];

    [JsonPropertyName("involved_companies")]
    public List<IgdbInvolvedCompanyRaw> InvolvedCompanies { get; set; } = [];
}

public class IgdbCoverRaw
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class IgdbImageRaw
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class IgdbNamedEntityRaw
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class IgdbInvolvedCompanyRaw
{
    [JsonPropertyName("company")]
    public IgdbNamedEntityRaw? Company { get; set; }
}
