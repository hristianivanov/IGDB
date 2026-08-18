namespace GameVault.Services.Models.Games;

public class GameDetailsDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string? CoverUrl { get; set; }

    public DateTime? FirstReleaseDate { get; set; }

    public double? Rating { get; set; }

    public int? RatingCount { get; set; }

    public IReadOnlyList<string> Genres { get; set; } = [];

    public IReadOnlyList<string> Platforms { get; set; } = [];

    public IReadOnlyList<string> Screenshots { get; set; } = [];

    public IReadOnlyList<string> InvolvedCompanies { get; set; } = [];
}
