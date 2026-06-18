using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(IProductRepository products, ICategoryRepository categories)
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!await categories.ExistsAsync(request.CategoryId, cancellationToken))
            return Result.Failure<Guid>(CategoryErrors.NotFound(request.CategoryId));

        var result = Product.Create(request.Name, request.Price, request.StockQuantity, request.CategoryId);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        var product = result.Value;

        await products.AddAsync(product, cancellationToken);
        await products.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
