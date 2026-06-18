using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace WebApi.IntegrationTests;

public class ProductsEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Creating_a_product_then_listing_returns_it()
    {
        var categoryId = await CreateCategoryAsync("Peripherals");

        var create = await _client.PostAsJsonAsync(
            "/api/products", new { name = "Keyboard", price = 45.00m, stockQuantity = 12, categoryId });

        create.StatusCode.ShouldBe(HttpStatusCode.Created);

        var page = await _client.GetFromJsonAsync<PagedProducts>("/api/products");

        page.ShouldNotBeNull();
        page!.TotalCount.ShouldBeGreaterThan(0);
        page.Items.ShouldContain(p =>
            p.Name == "Keyboard" && p.Price == 45.00m && p.StockQuantity == 12 && p.CategoryId == categoryId);
    }

    [Fact]
    public async Task Listing_products_honours_the_page_size()
    {
        var categoryId = await CreateCategoryAsync("Cables");
        foreach (var name in new[] { "Cable A", "Cable B", "Cable C" })
        {
            await _client.PostAsJsonAsync(
                "/api/products", new { name, price = 5.00m, stockQuantity = 1, categoryId });
        }

        var page = await _client.GetFromJsonAsync<PagedProducts>("/api/products?page=1&pageSize=2");

        page.ShouldNotBeNull();
        page!.Items.Count.ShouldBe(2);
        page.PageSize.ShouldBe(2);
        page.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task Creating_a_product_then_fetching_it_by_id_returns_it()
    {
        var categoryId = await CreateCategoryAsync("Audio");

        var create = await _client.PostAsJsonAsync(
            "/api/products", new { name = "Headset", price = 80.00m, stockQuantity = 5, categoryId });

        create.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<CreatedResponse>();
        created.ShouldNotBeNull();

        var product = await _client.GetFromJsonAsync<ProductResponse>($"/api/products/{created!.Id}");

        product.ShouldNotBeNull();
        product!.Id.ShouldBe(created.Id);
        product.Name.ShouldBe("Headset");
        product.CategoryId.ShouldBe(categoryId);
    }

    [Fact]
    public async Task Creating_a_product_for_a_missing_category_returns_404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            new { name = "Keyboard", price = 45.00m, stockQuantity = 12, categoryId = Guid.NewGuid() });

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Fetching_an_unknown_product_returns_404()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Health_endpoint_reports_healthy()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe("Healthy");
    }

    [Theory]
    [InlineData("", 45, 12)]
    [InlineData("Keyboard", -1, 12)]
    [InlineData("Keyboard", 45, -5)]
    public async Task Creating_a_product_with_an_invalid_payload_returns_400(
        string name, decimal price, int stockQuantity)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products", new { name, price, stockQuantity, categoryId = Guid.NewGuid() });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    private async Task<Guid> CreateCategoryAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new { name });
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        created.ShouldNotBeNull();
        return created!.Id;
    }

    private sealed record CreatedResponse(Guid Id);

    private sealed record ProductResponse(Guid Id, string Name, decimal Price, int StockQuantity, Guid CategoryId);

    private sealed record PagedProducts(List<ProductResponse> Items, int Page, int PageSize, int TotalCount);
}
