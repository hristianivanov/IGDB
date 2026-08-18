namespace GameVault.Services.Tests;

using GameVault.Common.Exceptions;
using GameVault.Contracts.Library;
using GameVault.Data.Models.Library;
using GameVault.Services.Library;
using GameVault.Services.Mapping;
using GameVault.Services.Models.Library;

public class LibraryServiceTests
{
    static LibraryServiceTests() => ServicesMappingConfig.Register();

    [Fact]
    public async Task AddAsync_RejectsInvalidStatus()
    {
        var service = CreateService();
        var request = ValidRequest();
        request.Status = "Backlog";

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.AddAsync(request, CancellationToken.None));

        Assert.Contains("Status must be one of", exception.Message);
    }

    [Fact]
    public async Task AddAsync_RejectsDuplicateGame()
    {
        var existing = new SavedGame { Id = Guid.NewGuid(), IgdbGameId = 42, Name = "Existing", Status = GameListStatus.Wishlist };
        var repository = new FakeSavedGameRepository([existing]);
        var service = CreateService(repository);

        var request = ValidRequest();
        request.IgdbGameId = 42;

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.AddAsync(request, CancellationToken.None));

        Assert.Contains("already in your library", exception.Message);
    }

    [Fact]
    public async Task AddAsync_AddsValidGame()
    {
        var repository = new FakeSavedGameRepository();
        var service = CreateService(repository);

        var result = await service.AddAsync(ValidRequest(), CancellationToken.None);

        Assert.Equal("The Witcher 3", result.Name);
        Assert.Equal("Wishlist", result.Status);
        Assert.Single(repository.Games);
    }

    [Fact]
    public async Task UpdateStatusAsync_ThrowsWhenMissing()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateStatusAsync(Guid.NewGuid(), new UpdateSavedGameRequestDto { Status = "Playing" }, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateStatusAsync_UpdatesStatus()
    {
        var existing = new SavedGame { Id = Guid.NewGuid(), IgdbGameId = 1, Name = "Hades", Status = GameListStatus.Wishlist };
        var repository = new FakeSavedGameRepository([existing]);
        var service = CreateService(repository);

        var result = await service.UpdateStatusAsync(existing.Id, new UpdateSavedGameRequestDto { Status = "Playing" }, CancellationToken.None);

        Assert.Equal("Playing", result.Status);
    }

    [Fact]
    public async Task DeleteAsync_RemovesGame()
    {
        var existing = new SavedGame { Id = Guid.NewGuid(), IgdbGameId = 1, Name = "Hades", Status = GameListStatus.Wishlist };
        var repository = new FakeSavedGameRepository([existing]);
        var service = CreateService(repository);

        await service.DeleteAsync(existing.Id, CancellationToken.None);

        Assert.Empty(repository.Games);
    }

    private static CreateSavedGameRequestDto ValidRequest() => new()
    {
        IgdbGameId = 1942,
        Name = "The Witcher 3",
        CoverUrl = "https://images.igdb.com/cover.jpg",
        Status = "Wishlist",
    };

    private static LibraryService CreateService(FakeSavedGameRepository? repository = null)
        => new(repository ?? new FakeSavedGameRepository());

    private sealed class FakeSavedGameRepository(List<SavedGame>? games = null) : ISavedGameRepository
    {
        public List<SavedGame> Games { get; } = games ?? [];

        public Task<IReadOnlyList<SavedGame>> GetAllAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<SavedGame>>(Games);

        public Task<SavedGame?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(Games.SingleOrDefault(g => g.Id == id));

        public Task<SavedGame?> GetByIgdbGameIdAsync(long igdbGameId, CancellationToken cancellationToken)
            => Task.FromResult(Games.SingleOrDefault(g => g.IgdbGameId == igdbGameId));

        public Task<SavedGame> AddAsync(SavedGame savedGame, CancellationToken cancellationToken)
        {
            Games.Add(savedGame);
            return Task.FromResult(savedGame);
        }

        public Task UpdateAsync(SavedGame savedGame, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task DeleteAsync(SavedGame savedGame, CancellationToken cancellationToken)
        {
            Games.Remove(savedGame);
            return Task.CompletedTask;
        }
    }
}
