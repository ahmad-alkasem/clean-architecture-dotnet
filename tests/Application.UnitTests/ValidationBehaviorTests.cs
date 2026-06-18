using Application.Common.Behaviors;
using Application.Products.Commands.CreateProduct;
using FluentValidation;
using MediatR;
using Shouldly;

namespace Application.UnitTests;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Throws_and_short_circuits_when_a_validator_fails()
    {
        var behavior = new ValidationBehavior<CreateProductCommand, Guid>(
            [new CreateProductCommandValidator()]);
        var nextInvoked = false;
        RequestHandlerDelegate<Guid> next = _ =>
        {
            nextInvoked = true;
            return Task.FromResult(Guid.NewGuid());
        };

        await Should.ThrowAsync<ValidationException>(
            () => behavior.Handle(new CreateProductCommand("", -1m, -1, Guid.Empty), next, CancellationToken.None));

        nextInvoked.ShouldBeFalse();
    }

    [Fact]
    public async Task Invokes_next_and_returns_its_result_when_validation_passes()
    {
        var behavior = new ValidationBehavior<CreateProductCommand, Guid>(
            Array.Empty<IValidator<CreateProductCommand>>());
        var expected = Guid.NewGuid();
        RequestHandlerDelegate<Guid> next = _ => Task.FromResult(expected);

        var result = await behavior.Handle(
            new CreateProductCommand("Keyboard", 45m, 12, Guid.NewGuid()), next, CancellationToken.None);

        result.ShouldBe(expected);
    }
}
