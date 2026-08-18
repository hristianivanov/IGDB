namespace GameVault.Services.Models.Games;

public class GameSummaryDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? CoverUrl { get; set; }

    public DateTime? FirstReleaseDate { get; set; }

    public double? Rating { get; set; }

    public IReadOnlyList<string> Genres { get; set; } = [];
}
