using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace WebApi.IntegrationTests;

public class CategoriesEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Creating_a_category_then_listing_returns_it()
    {
        var create = await _client.PostAsJsonAsync("/api/categories", new { name = "Storage" });

        create.StatusCode.ShouldBe(HttpStatusCode.Created);

        var categories = await _client.GetFromJsonAsync<List<CategoryResponse>>("/api/categories");

        categories.ShouldNotBeNull();
        categories!.ShouldContain(c => c.Name == "Storage");
    }

    [Fact]
    public async Task Creating_a_category_with_a_blank_name_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new { name = "" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    private sealed record CategoryResponse(Guid Id, string Name);
}
