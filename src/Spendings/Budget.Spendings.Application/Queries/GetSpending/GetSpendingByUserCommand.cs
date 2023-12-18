using Budget.Spendings.Domain.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpending;

public class GetSpendingsByUserCommand : IRequest<IEnumerable<Spending>>
{
    public GetSpendingsByUserCommand(
        string userId,
        DateTime? beginDate,
        DateTime? endDate
    )
    {
        UserId = userId;
        BeginDate = beginDate;
        EndDate = endDate;
    }

    public string UserId { get; }

    public Guid CategoryId { get; }

    public DateTime? BeginDate { get; }
    
    public DateTime? EndDate { get; }
}