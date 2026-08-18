namespace GameVault.Common.Exceptions;

public class IgdbUnavailableException(string message, Exception? inner = null)
    : Exception(message, inner);
