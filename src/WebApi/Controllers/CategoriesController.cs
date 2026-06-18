using Application.Categories.Commands.CreateCategory;
using Application.Categories.Dtos;
using Application.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/categories")]
public sealed class CategoriesController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> List(CancellationToken cancellationToken)
    {
        var categories = await sender.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? Created($"/api/categories/{result.Value}", new { id = result.Value })
            : Problem(result.Error);
    }
}
