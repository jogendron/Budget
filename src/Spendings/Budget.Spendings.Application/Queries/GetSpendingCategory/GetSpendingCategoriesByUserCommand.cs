using Budget.Spendings.Domain.Entities;

using System.ComponentModel.DataAnnotations;
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

    [Required]
    public string UserId { get; set; }
}