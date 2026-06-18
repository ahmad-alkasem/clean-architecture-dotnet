using Application.Categories.Dtos;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(ICategoryRepository repository)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.ListAsync(cancellationToken);
        return categories.Select(CategoryDto.FromEntity).ToList();
    }
}
