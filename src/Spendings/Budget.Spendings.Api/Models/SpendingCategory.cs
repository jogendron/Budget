using Budget.Spendings.Domain.Entities;

using System.ComponentModel.DataAnnotations;

namespace Budget.Spendings.Api.Models;

public class SpendingCategory
{
    public SpendingCategory(Domain.Entities.SpendingCategory spendingCategory)
    {
        Id = spendingCategory.Id;
        Name = spendingCategory.Name;
        Period = new Period(spendingCategory.Period);
        Frequency = spendingCategory.Frequency;
        Amount = spendingCategory.Amount;
        Description = spendingCategory.Description;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public Period Period { get; set; }

    [Required]
    public Frequency Frequency { get; set; }

    [Required]
    public double Amount { get; set; }

    [Required]
    public string Description { get; set; }
}