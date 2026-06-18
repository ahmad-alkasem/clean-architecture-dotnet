using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected IActionResult Problem(Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = TitleFor(error.Type),
            Detail = error.Description,
        };
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem) { StatusCode = status };
    }

    private static string TitleFor(ErrorType type) => type switch
    {
        ErrorType.Validation => "Validation failed",
        ErrorType.NotFound => "Resource not found",
        ErrorType.Conflict => "Conflict",
        _ => "Unexpected error",
    };
}
