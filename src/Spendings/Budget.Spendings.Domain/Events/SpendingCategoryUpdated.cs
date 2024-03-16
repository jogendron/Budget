using Budget.EventSourcing.Events;

namespace Budget.Spendings.Domain.Events;

public class SpendingCategoryUpdated : Event
{
    public SpendingCategoryUpdated() : base()
    {
        Name = string.Empty;
        Frequency = Frequency.Daily;
        Amount = 0;
        Description = string.Empty;
    }
    
    public SpendingCategoryUpdated(
        Guid aggregateId, 
        string name, 
        Frequency frequency,
        double amount,
        string description
    ) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
    {
        Name = name;
        Frequency = frequency;
        Amount = amount;
        Description = description;
    }

    public string Name { get; set; }

    public Frequency Frequency { get; set; }

    public double Amount { get; set; }

    public string Description { get; set; }
}