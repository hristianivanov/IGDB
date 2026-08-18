FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GameVault.slnx ./
COPY GameVault.Api/GameVault.Api.csproj GameVault.Api/
COPY GameVault.Services/GameVault.Services.csproj GameVault.Services/
COPY GameVault.Data/GameVault.Data.csproj GameVault.Data/
COPY GameVault.Infrastructure/GameVault.Infrastructure.csproj GameVault.Infrastructure/
COPY GameVault.Common/GameVault.Common.csproj GameVault.Common/
COPY GameVault.Contracts/GameVault.Contracts.csproj GameVault.Contracts/
COPY GameVault.Data.Models/GameVault.Data.Models.csproj GameVault.Data.Models/
COPY GameVault.Services.Models/GameVault.Services.Models.csproj GameVault.Services.Models/
RUN dotnet restore GameVault.Api/GameVault.Api.csproj

COPY . .
RUN dotnet publish GameVault.Api/GameVault.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
CMD ["sh", "-c", "dotnet GameVault.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
