using Application.Common.Models;
using Application.Products.Dtos;
using MediatR;

namespace Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<ProductDto>>;
