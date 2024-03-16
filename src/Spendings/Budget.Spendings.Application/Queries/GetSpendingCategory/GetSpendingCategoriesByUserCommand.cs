using Budget.Spendings.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoriesByUserCommand : IRequest<IEnumerable<SpendingCategory>>
{
    public GetSpendingCategoriesByUserCommand(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        UserId = userId;
    }

    [Required]
    public string UserId { get; set; }
}