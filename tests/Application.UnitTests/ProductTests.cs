using Domain.Entities;
using Domain.Entities.Events;
using Shouldly;

namespace Application.UnitTests;

public class ProductTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    [Fact]
    public void Create_with_valid_input_succeeds_and_trims_name()
    {
        var result = Product.Create("  Keyboard  ", 45.00m, 12, CategoryId);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldNotBe(Guid.Empty);
        result.Value.Name.ShouldBe("Keyboard");
        result.Value.Price.ShouldBe(45.00m);
        result.Value.StockQuantity.ShouldBe(12);
        result.Value.CategoryId.ShouldBe(CategoryId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_with_blank_name_fails(string? name)
    {
        var result = Product.Create(name!, 10m, 1, CategoryId);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(ProductErrors.NameRequired);
    }

    [Fact]
    public void Create_with_name_over_200_characters_fails()
    {
        var result = Product.Create(new string('x', 201), 10m, 1, CategoryId);

        result.Error.ShouldBe(ProductErrors.NameTooLong);
    }

    [Fact]
    public void Create_with_negative_price_fails()
    {
        var result = Product.Create("Mouse", -1m, 1, CategoryId);

        result.Error.ShouldBe(ProductErrors.NegativePrice);
    }

    [Fact]
    public void Create_with_negative_stock_fails()
    {
        var result = Product.Create("Mouse", 10m, -5, CategoryId);

        result.Error.ShouldBe(ProductErrors.NegativeStock);
    }

    [Fact]
    public void Create_raises_a_product_created_domain_event()
    {
        var product = Product.Create("Keyboard", 45m, 12, CategoryId).Value;

        var domainEvent = product.DomainEvents.OfType<ProductCreatedDomainEvent>().SingleOrDefault();

        domainEvent.ShouldNotBeNull();
        domainEvent!.ProductId.ShouldBe(product.Id);
        domainEvent.Name.ShouldBe("Keyboard");
    }
}
