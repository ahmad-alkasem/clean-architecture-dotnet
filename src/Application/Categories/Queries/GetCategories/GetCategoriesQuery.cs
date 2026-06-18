using Application.Categories.Dtos;
using Application.Common.Caching;
using MediatR;

namespace Application.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>, ICachedQuery
{
    public const string Key = "categories:all";

    public string CacheKey => Key;

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
