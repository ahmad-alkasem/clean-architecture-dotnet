using Domain.Entities;
using Shouldly;

namespace Application.UnitTests;

public class CategoryTests
{
    [Fact]
    public void Create_with_valid_name_succeeds_and_trims()
    {
        var result = Category.Create("  Peripherals  ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldNotBe(Guid.Empty);
        result.Value.Name.ShouldBe("Peripherals");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_with_blank_name_fails(string? name)
    {
        var result = Category.Create(name!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NameRequired);
    }

    [Fact]
    public void Create_with_name_over_100_characters_fails()
    {
        var result = Category.Create(new string('x', 101));

        result.Error.ShouldBe(CategoryErrors.NameTooLong);
    }
}
