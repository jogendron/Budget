using Budget.Spendings.Domain.Entities;

using System.ComponentModel.DataAnnotations;
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

    [Required]    
    public string UserId { get; set; }

    [Required]
    public string Name { get; set; }
}