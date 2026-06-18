using Domain.Entities;

namespace Application.Categories.Dtos;

public sealed record CategoryDto(Guid Id, string Name)
{
    public static CategoryDto FromEntity(Category c) => new(c.Id, c.Name);
}
