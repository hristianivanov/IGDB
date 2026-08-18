namespace GameVault.Services.Models.Library;

public class CreateSavedGameRequestDto
{
    public long IgdbGameId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? CoverUrl { get; set; }

    public string Status { get; set; } = string.Empty;
}
