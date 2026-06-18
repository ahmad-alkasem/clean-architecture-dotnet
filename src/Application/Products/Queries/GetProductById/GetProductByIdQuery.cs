using Application.Products.Dtos;
using Domain.Common;
using MediatR;

namespace Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
