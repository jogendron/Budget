using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Domain.Entities;

using System.ComponentModel.DataAnnotations;

namespace Budget.Spendings.Api.Models;

public class NewSpendingCategory
{
    public NewSpendingCategory()
    {
        Name = string.Empty;
        Frequency = Frequency.Daily;
        Amount = 0;
        Description = string.Empty;
    }

    [Required]
    public string Name { get; set; }

    [Required]
    public Frequency Frequency { get; set; }

    [Required]
    public double Amount { get; set; }

    [Required]
    public string Description { get; set; }

    internal CreateSpendingCategoryCommand ToCreateCommand(string userId)
    {
        return new CreateSpendingCategoryCommand(
            userId,
            Name,
            Frequency,
            Amount,
            Description
        );
    }
}