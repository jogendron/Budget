using MediatR;

namespace Budget.Spendings.Application.Commands.DeleteSpendingCategory;

public class DeleteSpendingCategoryCommand : IRequest
{
    public DeleteSpendingCategoryCommand(
        Guid id,
        string userId
    )
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty");

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        Id = id;
        UserId = userId;
    }

    public Guid Id { get; }

    public string UserId { get; }
}