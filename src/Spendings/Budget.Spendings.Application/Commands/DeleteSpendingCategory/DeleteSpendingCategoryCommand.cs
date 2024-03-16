using MediatR;

namespace Budget.Spendings.Application.Commands.DeleteSpendingCategory;

public class DeleteSpendingCategoryCommand : IRequest
{
    public DeleteSpendingCategoryCommand(
        string userId,
        Guid id
    )
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty");

        UserId = userId;
        Id = id;
    }

    public string UserId { get; }

    public Guid Id { get; }
}