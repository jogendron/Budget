namespace Budget.Spendings.Infrastructure.EF;

public class SpendingCategory
{
    public SpendingCategory()
    {
        Id = Guid.Empty;
        UserId = string.Empty;
        Name = string.Empty;
        BeginDate = DateTime.MinValue;
        ModifiedOn = DateTime.MinValue;
        EndDate = null;
        Frequency = Frequency.Monthly;
        Amount = 0;
        Description = string.Empty;
        Events = new List<Event>();
    }

    public SpendingCategory(Budget.Spendings.Domain.WriteModel.Entities.SpendingCategory category)
    {
        Id = category.Id;
        UserId = category.UserId;
        Name = category.Name;
        BeginDate = category.Period.BeginDate;
        ModifiedOn = category.Changes.Max(c => c.EventDate);
        EndDate = category.Period.EndDate;
        Frequency = category.Frequency.ToDbFrequency();
        Amount = category.Amount;
        Description = category.Description;
        Events = category.Changes.Select(c => new Event(c)).ToList();
    }

    public Guid Id { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    public DateTime BeginDate { get; set; }

    public DateTime ModifiedOn { get; set; }

    public DateTime? EndDate { get; set; }

    public Frequency Frequency { get; set; }

    public double Amount { get; set; }

    public string Description { get; set; }

    public List<Event> Events { get; set; }

    internal Domain.ReadModel.Entities.SpendingCategory ToReadModel()
    {
        return new Domain.ReadModel.Entities.SpendingCategory(
            Id,
            UserId,
            Name,
            BeginDate,
            ModifiedOn,
            EndDate,
            Frequency.ToReadModel(),
            Amount,
            Description
        );
    }

}