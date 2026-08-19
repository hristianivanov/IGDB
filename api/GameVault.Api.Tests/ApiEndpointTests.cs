namespace GameVault.Api.Tests;

using System.Net;
using System.Net.Http.Json;

using GameVault.Services.Models.Library;

public class ApiEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Library_AddThenGetThenDelete_RoundTrips()
    {
        var createRequest = new CreateSavedGameRequestDto
        {
            ExternalGameId = 1942,
            Name = "The Witcher 3",
            CoverUrl = "https://media.rawg.io/media/cover.jpg",
            Status = "Wishlist",
        };

        var createResponse = await _client.PostAsJsonAsync("/api/library", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<SavedGameDto>();
        Assert.NotNull(created);
        Assert.Equal("Wishlist", created!.Status);

        var listResponse = await _client.GetAsync("/api/library");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<List<SavedGameDto>>();
        Assert.Single(list!);

        var deleteResponse = await _client.DeleteAsync($"/api/library/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
