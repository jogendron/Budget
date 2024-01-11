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
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        if (beginDate.HasValue && endDate.HasValue && beginDate > endDate)
            throw new ArgumentException("End date must be after begin date");

        UserId = userId;
        BeginDate = beginDate;
        EndDate = endDate;
    }

    public string UserId { get; }

    public DateTime? BeginDate { get; }
    
    public DateTime? EndDate { get; }
}