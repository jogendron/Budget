using MediatR;

namespace Budget.Spendings.Application.Commands.DeleteSpendings;

public class DeleteSpendingsCommand : IRequest
{
    public DeleteSpendingsCommand(
        IEnumerable<Guid> spendingIds,
        string userId
    )
    {
        SpendingIds = spendingIds;
        UserId = userId;
    }

    public IEnumerable<Guid> SpendingIds { get; }

    public string UserId { get; }
}