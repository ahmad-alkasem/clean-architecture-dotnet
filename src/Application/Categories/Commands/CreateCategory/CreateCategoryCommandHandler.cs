using Application.Categories.Queries.GetCategories;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(ICategoryRepository repository, IMemoryCache cache)
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var result = Category.Create(request.Name);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        var category = result.Value;

        await repository.AddAsync(category, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        cache.Remove(GetCategoriesQuery.Key);

        return category.Id;
    }
}
