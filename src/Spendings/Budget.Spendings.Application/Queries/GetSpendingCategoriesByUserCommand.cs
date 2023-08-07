using Budget.Spendings.Domain.ReadModel.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoriesByUserCommand : IRequest<IEnumerable<SpendingCategory>>
{
    public GetSpendingCategoriesByUserCommand()
    {
        UserId = string.Empty;
    }

    public GetSpendingCategoriesByUserCommand(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; set; }
}