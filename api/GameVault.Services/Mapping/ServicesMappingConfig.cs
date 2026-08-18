namespace GameVault.Services.Mapping;

using Mapster;

using GameVault.Data.Models.Library;
using GameVault.Services.Models.Library;

public static class ServicesMappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<SavedGame, SavedGameDto>.NewConfig()
            .Map(dest => dest.Status, src => src.Status.ToString());
    }
}
