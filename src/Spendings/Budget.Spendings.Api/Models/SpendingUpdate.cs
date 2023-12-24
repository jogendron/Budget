using System.ComponentModel.DataAnnotations;

namespace Budget.Spendings.Api.Models;

public class SpendingUpdate
{
    public SpendingUpdate()
    {
        Id = Guid.Empty;
        CategoryId = null;
        Date = null;
        Amount = null;
        Description = null;
    }

    public SpendingUpdate(
        Guid id,
        Guid categoryId,
        DateTime? date,
        double? amount,
        string? description
    )
    {
        Id = id;
        CategoryId = categoryId;
        Date = date;
        Amount = amount;
        Description = description;        
    }

    [Required]
    public Guid Id { get; set; }

    public Guid? CategoryId { get; set; }

    public DateTime? Date { get; set; }

    public double? Amount { get; set; }

    public string? Description { get; set; }
}