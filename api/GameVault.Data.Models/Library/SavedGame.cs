namespace GameVault.Data.Models.Library;

public class SavedGame
{
    public Guid Id { get; set; }

    public long ExternalGameId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? CoverUrl { get; set; }

    public GameListStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
