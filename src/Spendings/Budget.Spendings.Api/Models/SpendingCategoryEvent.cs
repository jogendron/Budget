using System.ComponentModel.DataAnnotations;

namespace Budget.Spendings.Api.Models;

public class SpendingCategoryEvent
{
    public SpendingCategoryEvent()
    {
        EventType = string.Empty;
    }

    [Required]
    public Guid EventId { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public string EventType { get; set; }
}