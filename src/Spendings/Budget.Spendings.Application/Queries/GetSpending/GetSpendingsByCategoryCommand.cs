using Budget.Spendings.Domain.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpending;

public class GetSpendingsByCategoryCommand : IRequest<IEnumerable<Spending>>
{
    public GetSpendingsByCategoryCommand(
        string userId,
        Guid categoryId,
        DateTime? beginDate,
        DateTime? endDate
    )
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category id cannot be empty");

        if (beginDate != null && endDate != null && beginDate > endDate)
            throw new ArgumentException("End date must be after begin date");

        UserId = userId;
        CategoryId = categoryId;
        BeginDate = beginDate;
        EndDate = endDate;
    }

    public string UserId { get; }

    public Guid CategoryId { get; }

    public DateTime? BeginDate { get; }
    
    public DateTime? EndDate { get; }
}