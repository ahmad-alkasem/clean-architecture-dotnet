using Domain.Common;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, decimal Price, int StockQuantity, Guid CategoryId)
    : IRequest<Result<Guid>>;
