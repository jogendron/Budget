using Budget.Spendings.Domain.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpending;

public class GetSpendingByIdCommand : IRequest<Spending?>
{
    public GetSpendingByIdCommand(Guid id, string userId)
    {
        Id = id;
        UserId = userId;
    }

    public Guid Id { get; }

    public string UserId { get; }
}