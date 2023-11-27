using System.ComponentModel.DataAnnotations;

namespace Budget.Spendings.Api.Models;

public class Spending
{
    public Spending()
    {
        Id = Guid.Empty;
        CategoryId = Guid.Empty;
        Date = DateTime.MinValue;
        Amount = 0;
        Description = string.Empty;
    }

    public Spending(Domain.Entities.Spending spending)
    {
        Id = spending.Id;
        CategoryId = spending.CategoryId;
        Date = spending.Date;
        Amount = spending.Amount;
        Description = spending.Description;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public double Amount { get; set; }

    [Required]
    public string Description { get; set; }
}