using Domain.Common;

namespace Domain.Entities;

public static class ProductErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Product.NameRequired", "Product name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Product.NameTooLong", "Product name must not exceed 200 characters.");

    public static readonly Error NegativePrice =
        Error.Validation("Product.NegativePrice", "Price cannot be negative.");

    public static readonly Error NegativeStock =
        Error.Validation("Product.NegativeStock", "Stock cannot be negative.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Product.NotFound", $"Product '{id}' was not found.");
}
