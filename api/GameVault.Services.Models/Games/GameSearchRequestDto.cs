namespace GameVault.Services.Models.Games;

public class GameSearchRequestDto
{
    public string? Query { get; set; }

    public int Limit { get; set; } = 20;

    public int Offset { get; set; }
}
