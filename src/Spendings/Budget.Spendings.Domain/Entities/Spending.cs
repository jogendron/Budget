using System.Diagnostics.CodeAnalysis;
using Budget.EventSourcing.Entities;
using Budget.EventSourcing.Events;
using Budget.Spendings.Domain.Events;

namespace Budget.Spendings.Domain.Entities;

public class Spending 
    : Aggregate,
    IEventHandler<SpendingCreated>,
    IEventHandler<SpendingUpdated>
{
    private Guid _categoryId;
    private DateTime _date;
    private double _amount;
    private string _description;

    internal Spending(
        Guid categoryId, 
        DateTime date, 
        double amount, 
        string description
    ) : this(Guid.Empty, new List<Event>())
    {
        Id = Guid.NewGuid();

        AddChange(
            new SpendingCreated(
                Id,
                categoryId,
                date,
                amount,
                description
            )
        );
    }
    
    internal Spending(Guid id, IEnumerable<Event> changes) : base(id, changes)
    {
        InitializeMembers();
    }

    public Guid CategoryId
    {
        get { return _categoryId; }
        private set {
            if (value == Guid.Empty)
                throw new ArgumentException("Category id cannot be empty.");

            _categoryId = value;
        }
    }

    public DateTime Date 
    {
        get {
            return _date;
        }
        private set {
            _date = value;
        }
    }

    public double Amount
    {
        get {
            return _amount;
        }
        private set {
            if (value < 0)
                throw new ArgumentException("Amount cannot be lower than 0.");

            _amount = value;
        }
    }

    public string Description
    {
        get {
            return _description;
        }
        private set {
            _description = value;
        }
    }

    public void Update(Guid categoryId, DateTime date, double amount, string description)
    {
        AddChange(
            new SpendingUpdated(
                Id,
                categoryId,
                date,
                amount,
                description
            )
        );
    }

    [MemberNotNull(nameof(_date))]
    [MemberNotNull(nameof(_amount))]
    [MemberNotNull(nameof(_description))]
    protected override void InitializeMembers()
    {
        _date = DateTime.MinValue;
        _amount = 0;
        _description = string.Empty;
    }

    void IEventHandler<SpendingCreated>.Handle(SpendingCreated @event)
    {
        Id = @event.AggregateId;

        _categoryId = @event.CategoryId;
        Date = @event.Date;
        Amount = @event.Amount;
        Description = @event.Description;
    }

    void IEventHandler<SpendingUpdated>.Handle(SpendingUpdated @event)
    {
        CategoryId = @event.CategoryId;
        Date = @event.Date;
        Amount = @event.Amount;
        Description = @event.Description;
    }
}