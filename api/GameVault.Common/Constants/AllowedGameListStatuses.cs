namespace GameVault.Common.Constants;

public static class AllowedGameListStatuses
{
    public static readonly HashSet<string> Values = new(StringComparer.OrdinalIgnoreCase)
    {
        "Wishlist",
        "Playing",
        "Completed"
    };
}
