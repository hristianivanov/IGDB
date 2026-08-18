namespace GameVault.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using GameVault.Contracts.Games;
using GameVault.Infrastructure.ExternalApis.Igdb;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<IgdbOptions>(configuration.GetSection(IgdbOptions.SectionName));

        services.AddHttpClient("TwitchAuth", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddHttpClient("Igdb", client =>
        {
            client.BaseAddress = new Uri("https://api.igdb.com/v4/");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddSingleton<ITwitchTokenProvider, TwitchTokenProvider>();
        services.AddScoped<IIgdbClient, IgdbClient>();

        return services;
    }
}
