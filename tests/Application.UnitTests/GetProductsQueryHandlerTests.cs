using Application.Common.Interfaces;
using Application.Products.Queries.GetProducts;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Application.UnitTests;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_maps_entities_and_reports_paging_metadata()
    {
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            Product.Create("Keyboard", 45m, 12, categoryId).Value,
            Product.Create("Mouse", 20m, 30, categoryId).Value,
        };
        var repository = Substitute.For<IProductRepository>();
        repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Product>)products, 5));

        var handler = new GetProductsQueryHandler(repository);

        var result = await handler.Handle(new GetProductsQuery(1, 10), CancellationToken.None);

        result.Items.Count.ShouldBe(2);
        result.Items[0].Name.ShouldBe("Keyboard");
        result.Page.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.TotalCount.ShouldBe(5);
        result.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_clamps_out_of_range_paging_arguments()
    {
        var repository = Substitute.For<IProductRepository>();
        repository.GetPagedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Product>)Array.Empty<Product>(), 0));

        var handler = new GetProductsQueryHandler(repository);

        var result = await handler.Handle(new GetProductsQuery(0, 0), CancellationToken.None);

        result.Page.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        await repository.Received(1).GetPagedAsync(1, 10, Arg.Any<CancellationToken>());
    }
}
