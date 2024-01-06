using MediatR;

namespace Budget.Spendings.Application.Commands.DeleteSpendingCategory;

public class DeleteSpendingCategoryCommand : IRequest
{
    public DeleteSpendingCategoryCommand(
        Guid id,
        string userId
    )
    {
        Id = id;
        UserId = userId;
    }

    public Guid Id { get; }

    public string UserId { get; }
}