using Budget.Spendings.Domain.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoryByIdCommand : IRequest<SpendingCategory?>
{
    public GetSpendingCategoryByIdCommand(Guid id, string userId)
    {
        Id = id;
        UserId = userId;
    }
   
    public Guid Id { get; }

    public string UserId { get; }
}