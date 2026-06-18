using Application.Categories.Commands.CreateCategory;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Shouldly;

namespace Application.UnitTests;

public class CreateCategoryCommandHandlerTests
{
    private static MemoryCache NewCache() => new(new MemoryCacheOptions());

    [Fact]
    public async Task Handle_persists_category_and_returns_its_id()
    {
        var repository = Substitute.For<ICategoryRepository>();
        Category? persisted = null;
        repository
            .When(r => r.AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>()))
            .Do(call => persisted = call.Arg<Category>());

        var handler = new CreateCategoryCommandHandler(repository, NewCache());

        var result = await handler.Handle(new CreateCategoryCommand("Peripherals"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        persisted.ShouldNotBeNull();
        persisted!.Name.ShouldBe("Peripherals");
        result.Value.ShouldBe(persisted.Id);

        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_returns_failure_and_does_not_persist_when_name_is_blank()
    {
        var repository = Substitute.For<ICategoryRepository>();
        var handler = new CreateCategoryCommandHandler(repository, NewCache());

        var result = await handler.Handle(new CreateCategoryCommand("  "), CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NameRequired);

        await repository.DidNotReceive().AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
