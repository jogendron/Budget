using System.ComponentModel.DataAnnotations;
using Budget.Spendings.Domain.Factories;

namespace Budget.Spendings.Infrastructure.EF;

public class Spending
{
    private DateTime _date;

    public Spending()
    {
        Id = Guid.Empty;
        SpendingCategoryId = Guid.Empty;
        Date = DateTime.MinValue;
        Amount = 0;
        Description = string.Empty;
        Events = new List<Event>();
    }

    public Spending(Domain.Entities.Spending spending)
    {
        Id = spending.Id;
        SpendingCategoryId = spending.CategoryId;
        Date = spending.Date;
        Amount = spending.Amount;
        Description = spending.Description;
        Events = spending.Changes.Select(c => new Event(c)).ToList();
    }

    public Guid Id { get; set; }

    public Guid SpendingCategoryId { get; set; }

    public DateTime Date 
    { 
        get => _date; 
        set {
            _date = DateTime.SpecifyKind(value.ToUniversalTime(), DateTimeKind.Utc);
        }
    }

    public double Amount { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    public List<Event> Events { get; set; }

    public Domain.Entities.Spending ToDomain()
    {
        var factory = new SpendingFactory();
        
        return factory.Load(Id, Events.Select(e => e.ToDomain()).OrderBy(e => e.EventDate));
    }
}