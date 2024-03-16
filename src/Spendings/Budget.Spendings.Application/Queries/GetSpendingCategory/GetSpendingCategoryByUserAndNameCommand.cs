using Budget.Spendings.Domain.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoryByUserAndNameCommand : IRequest<SpendingCategory?>
{
    public GetSpendingCategoryByUserAndNameCommand(string userId, string name)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be empty");

        UserId = userId;
        Name = name;
    }

    public string UserId { get; set; }

    public string Name { get; set; }
}