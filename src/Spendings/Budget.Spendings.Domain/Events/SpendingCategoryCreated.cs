using Budget.EventSourcing.Events;

namespace Budget.Spendings.Domain.Events;

public class SpendingCategoryCreated : Event
{
    public SpendingCategoryCreated() : base()
    {
        UserId = string.Empty;
        Name = string.Empty;
        Period = new Period(DateTime.Now);
        Frequency = Frequency.Daily;
        Amount = 0;
        Description = string.Empty;
    }

    public SpendingCategoryCreated(
        Guid aggregateId,
        string userId,
        string name,
        Period period,
        Frequency frequency,
        double amount,
        string description
    ) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
    {
        UserId = userId;
        Name = name;
        Period = period;
        Frequency = frequency;
        Amount = amount;
        Description = description;
    }

    public string UserId { get; set; }

    public string Name { get; set; }

    public Period Period { get; set; }

    public Frequency Frequency { get; set; }

    public double Amount { get; set; }
    
    public string Description { get; set; }
}