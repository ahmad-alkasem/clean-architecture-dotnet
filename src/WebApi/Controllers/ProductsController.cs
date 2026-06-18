using Application.Common.Models;
using Application.Products.Commands.CreateProduct;
using Application.Products.Dtos;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/products")]
public sealed class ProductsController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> List(
        int? page, int? pageSize, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductsQuery(page ?? 1, pageSize ?? 10), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? Created($"/api/products/{result.Value}", new { id = result.Value })
            : Problem(result.Error);
    }
}
