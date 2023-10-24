using System.ComponentModel.DataAnnotations;
using Budget.Spendings.Domain.Entities;

namespace Budget.Spendings.Api.Models;

public class SpendingCategoryUpdate
{
    public SpendingCategoryUpdate()
    {
        Id = Guid.Empty;
    }

    [Required]
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Frequency? Frequency { get; set; }

    public bool? IsPeriodOpened { get; set; }

    public double? Amount { get; set; }

    public string? Description { get; set; }
}