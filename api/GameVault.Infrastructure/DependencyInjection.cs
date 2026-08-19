namespace GameVault.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using GameVault.Contracts.Games;
using GameVault.Infrastructure.ExternalApis.Rawg;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RawgOptions>(configuration.GetSection(RawgOptions.SectionName));

        services.AddHttpClient("Rawg", client =>
        {
            client.BaseAddress = new Uri("https://api.rawg.io/api/");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddScoped<IGameCatalogClient, RawgClient>();

        return services;
    }
}
