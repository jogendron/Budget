using System.ComponentModel.DataAnnotations;
using Budget.Spendings.Domain.Factories;

namespace Budget.Spendings.Infrastructure.EF;

public class SpendingCategory
{ 
    private DateTime _beginDate = DateTime.MinValue;
    private DateTime _modifiedOn;
    private DateTime? _endDate;

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
        Spendings = new List<Spending>();
    }

    public SpendingCategory(Domain.Entities.SpendingCategory category)
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
        Spendings = new List<Spending>();
    }

    public Guid Id { get; set; }

    [MaxLength(100)]
    public string UserId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    public DateTime BeginDate 
    { 
        get => _beginDate;
        set {
            _beginDate = DateTime.SpecifyKind(value.ToUniversalTime(), DateTimeKind.Utc);
        }
    }

    public DateTime ModifiedOn 
    { 
        get => _modifiedOn;
        set {
            _modifiedOn = DateTime.SpecifyKind(value.ToUniversalTime(), DateTimeKind.Utc);
        }
    }

    public DateTime? EndDate 
    { 
        get => _endDate; 
        set {
            _endDate = value.HasValue 
                ? DateTime.SpecifyKind(value.Value.ToUniversalTime(), DateTimeKind.Utc)
                : null;
        }
    }

    public Frequency Frequency { get; set; }

    public double Amount { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    public List<Event> Events { get; set; }

    public List<Spending> Spendings { get; set; }

    internal Domain.Entities.SpendingCategory ToDomain()
    {
        var factory = new SpendingCategoryFactory();
        
        return factory.Load(Id, Events.Select(e => e.ToDomain()).OrderBy(e => e.EventDate));
    }

}