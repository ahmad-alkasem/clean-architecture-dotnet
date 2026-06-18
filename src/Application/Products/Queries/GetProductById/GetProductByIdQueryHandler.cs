using Application.Common.Interfaces;
using Application.Products.Dtos;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository repository)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);

        return product is null
            ? Result.Failure<ProductDto>(ProductErrors.NotFound(request.Id))
            : ProductDto.FromEntity(product);
    }
}
