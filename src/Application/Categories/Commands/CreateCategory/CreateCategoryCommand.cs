using Domain.Common;
using MediatR;

namespace Application.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(string Name) : IRequest<Result<Guid>>;
