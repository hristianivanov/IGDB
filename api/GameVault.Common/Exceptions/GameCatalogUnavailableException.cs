namespace GameVault.Common.Exceptions;

public class GameCatalogUnavailableException(string message, Exception? inner = null)
    : Exception(message, inner);
