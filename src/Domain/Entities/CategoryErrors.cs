using Domain.Common;

namespace Domain.Entities;

public static class CategoryErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Category.NameRequired", "Category name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Category.NameTooLong", "Category name must not exceed 100 characters.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Category.NotFound", $"Category '{id}' was not found.");
}
