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