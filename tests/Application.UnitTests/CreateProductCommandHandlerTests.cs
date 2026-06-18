using Application.Common.Interfaces;
using Application.Products.Commands.CreateProduct;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Application.UnitTests;

public class CreateProductCommandHandlerTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    [Fact]
    public async Task Handle_persists_product_and_returns_its_id()
    {
        var products = Substitute.For<IProductRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        categories.ExistsAsync(CategoryId, Arg.Any<CancellationToken>()).Returns(true);

        Product? persisted = null;
        products
            .When(r => r.AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>()))
            .Do(call => persisted = call.Arg<Product>());

        var handler = new CreateProductCommandHandler(products, categories);

        var result = await handler.Handle(
            new CreateProductCommand("Keyboard", 45m, 12, CategoryId), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        persisted.ShouldNotBeNull();
        persisted!.Name.ShouldBe("Keyboard");
        persisted.CategoryId.ShouldBe(CategoryId);
        result.Value.ShouldBe(persisted.Id);

        await products.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await products.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_returns_not_found_when_category_does_not_exist()
    {
        var products = Substitute.For<IProductRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        categories.ExistsAsync(CategoryId, Arg.Any<CancellationToken>()).Returns(false);

        var handler = new CreateProductCommandHandler(products, categories);

        var result = await handler.Handle(
            new CreateProductCommand("Keyboard", 45m, 12, CategoryId), CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NotFound(CategoryId));

        await products.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await products.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_returns_failure_and_does_not_persist_when_domain_rejects_input()
    {
        var products = Substitute.For<IProductRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        categories.ExistsAsync(CategoryId, Arg.Any<CancellationToken>()).Returns(true);

        var handler = new CreateProductCommandHandler(products, categories);

        var result = await handler.Handle(
            new CreateProductCommand("Mouse", -1m, 1, CategoryId), CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(ProductErrors.NegativePrice);

        await products.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await products.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
