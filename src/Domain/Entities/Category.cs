using Domain.Common;

namespace Domain.Entities;

public sealed class Category : AuditableEntity
{
    private Category() { }

    public string Name { get; private set; } = default!;

    public static Result<Category> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CategoryErrors.NameRequired;

        var trimmed = name.Trim();

        if (trimmed.Length > 100)
            return CategoryErrors.NameTooLong;

        return new Category { Name = trimmed };
    }
}
