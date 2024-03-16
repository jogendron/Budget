using System.ComponentModel.DataAnnotations;

namespace Budget.Spendings.Api.Models;

public class NewSpending
{
    public NewSpending()
    {
        CategoryId = Guid.Empty;
        Date = DateTime.MinValue;
        Amount = 0;
        Description = string.Empty;
    }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public double Amount { get; set; }

    [Required]
    public string Description { get; set; }
}