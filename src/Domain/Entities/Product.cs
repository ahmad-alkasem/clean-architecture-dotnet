using Domain.Common;
using Domain.Entities.Events;

namespace Domain.Entities;

public sealed class Product : AuditableEntity
{
    private Product() { }

    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public Guid CategoryId { get; private set; }

    public static Result<Product> Create(string name, decimal price, int stockQuantity, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ProductErrors.NameRequired;

        var trimmed = name.Trim();

        if (trimmed.Length > 200)
            return ProductErrors.NameTooLong;

        if (price < 0)
            return ProductErrors.NegativePrice;

        if (stockQuantity < 0)
            return ProductErrors.NegativeStock;

        var product = new Product
        {
            Name = trimmed,
            Price = price,
            StockQuantity = stockQuantity,
            CategoryId = categoryId,
        };

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id, product.Name));

        return product;
    }
}
