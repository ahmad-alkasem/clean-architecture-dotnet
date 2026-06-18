using Application.Products.Commands.CreateProduct;
using FluentValidation.TestHelper;

namespace Application.UnitTests;

public class CreateProductCommandValidatorTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Valid_command_has_no_errors()
    {
        var result = _validator.TestValidate(new CreateProductCommand("Keyboard", 45m, 12, CategoryId));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_name_is_rejected()
    {
        var result = _validator.TestValidate(new CreateProductCommand("", 45m, 12, CategoryId));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_longer_than_200_is_rejected()
    {
        var result = _validator.TestValidate(new CreateProductCommand(new string('x', 201), 45m, 12, CategoryId));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Negative_price_is_rejected()
    {
        var result = _validator.TestValidate(new CreateProductCommand("Keyboard", -1m, 12, CategoryId));

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Negative_stock_is_rejected()
    {
        var result = _validator.TestValidate(new CreateProductCommand("Keyboard", 45m, -1, CategoryId));

        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Fact]
    public void Empty_category_is_rejected()
    {
        var result = _validator.TestValidate(new CreateProductCommand("Keyboard", 45m, 12, Guid.Empty));

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }
}
