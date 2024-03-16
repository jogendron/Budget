using Budget.Spendings.Domain.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpending;

public class GetSpendingByIdCommand : IRequest<Spending?>
{
    public GetSpendingByIdCommand(string userId, Guid id)
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