using Budget.EventSourcing.Events;

namespace Budget.Spendings.Domain.Events;

public class SpendingUpdated : Event
{
    public SpendingUpdated() : base()
    {
        CategoryId = Guid.Empty;
        Date = DateTime.MinValue;
        Amount = 0;
        Description = String.Empty;
    }

    public SpendingUpdated(
        Guid aggregateId, 
        Guid categoryId,
        DateTime date, 
        double amount, 
        string desription
    ) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
    {
        CategoryId = categoryId;
        Date = date;
        Amount = amount;
        Description = desription;
    }

    public Guid CategoryId { get; set; }

    public DateTime Date { get; set; }

    public double Amount { get; set; }

    public string Description { get; set; }
}