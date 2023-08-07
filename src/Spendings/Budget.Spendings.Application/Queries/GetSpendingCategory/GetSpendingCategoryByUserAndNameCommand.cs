using Budget.Spendings.Domain.ReadModel.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoryByUserAndNameCommand : IRequest<SpendingCategory?>
{
    public GetSpendingCategoryByUserAndNameCommand()
    {
        UserId = string.Empty;
        Name = string.Empty;
    }

    public GetSpendingCategoryByUserAndNameCommand(string userId, string name)
    {
        UserId = userId;
        Name = name;
    }
    
    public string UserId { get; set; }

    public string Name { get; set; }
}