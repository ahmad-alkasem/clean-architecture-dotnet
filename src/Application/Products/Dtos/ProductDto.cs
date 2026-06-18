using Domain.Entities;

namespace Application.Products.Dtos;

public sealed record ProductDto(Guid Id, string Name, decimal Price, int StockQuantity, Guid CategoryId)
{
    public static ProductDto FromEntity(Product p) => new(p.Id, p.Name, p.Price, p.StockQuantity, p.CategoryId);
}
