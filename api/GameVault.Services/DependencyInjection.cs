namespace GameVault.Services;

using Microsoft.Extensions.DependencyInjection;

using GameVault.Services.Games;
using GameVault.Services.Library;
using GameVault.Services.Mapping;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ServicesMappingConfig.Register();

        services.AddScoped<GameService>();
        services.AddScoped<LibraryService>();

        return services;
    }
}
