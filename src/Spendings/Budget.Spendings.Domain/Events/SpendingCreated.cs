using Budget.EventSourcing.Events;

namespace Budget.Spendings.Domain.Events;

public class SpendingCreated : Event
{
    public SpendingCreated()
    {
        CategoryId = Guid.Empty;
        Date = DateTime.MinValue;
        Amount = 0;
        Description = string.Empty;
    }

    public SpendingCreated(
        Guid aggregateId,
        Guid categoryId,
        DateTime date,
        double amount,
        string description
    ) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
    {
        CategoryId = categoryId;
        Date = date;
        Amount = amount;
        Description = description;        
    }

    public Guid CategoryId { get; set; }

    public DateTime Date { get; set; }

    public double Amount { get; set; }

    public string Description { get; set; }
}