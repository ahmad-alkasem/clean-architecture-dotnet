using Application.Common.Interfaces;
using Application.Products.Queries.GetProductById;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Application.UnitTests;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_returns_product_when_it_exists()
    {
        var product = Product.Create("Keyboard", 45m, 12, Guid.NewGuid()).Value;
        var repository = Substitute.For<IProductRepository>();
        repository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var handler = new GetProductByIdQueryHandler(repository);

        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(product.Id);
        result.Value.Name.ShouldBe("Keyboard");
    }

    [Fact]
    public async Task Handle_returns_not_found_when_product_is_missing()
    {
        var id = Guid.NewGuid();
        var repository = Substitute.For<IProductRepository>();
        repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var handler = new GetProductByIdQueryHandler(repository);

        var result = await handler.Handle(new GetProductByIdQuery(id), CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(ProductErrors.NotFound(id));
    }
}
