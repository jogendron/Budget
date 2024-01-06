using MediatR;

namespace Budget.Spendings.Application.Commands.DeleteSpending;

public class DeleteSpendingCommand : IRequest
{
    public DeleteSpendingCommand(
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