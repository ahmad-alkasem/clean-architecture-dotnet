using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Products.Dtos;
using MediatR;

namespace Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler(IProductRepository repository)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

        var (products, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var items = products.Select(ProductDto.FromEntity).ToList();

        return new PagedResult<ProductDto>(items, page, pageSize, totalCount);
    }
}
