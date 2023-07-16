using System.Diagnostics.CodeAnalysis;

using Budget.EventSourcing.Entities;
using Budget.EventSourcing.Events;
using Budget.Spendings.Domain.WriteModel.Events;

namespace Budget.Spendings.Domain.WriteModel.Entities;

public class SpendingCategory : 
        Aggregate,
        IEventHandler<SpendingCategoryCreated>,
        IEventHandler<SpendingCategoryUpdated>,
        IEventHandler<SpendingCategoryPeriodClosed>,
        IEventHandler<SpendingCategoryPeriodOpened>
{
    private string _userId;
    private string _name;
    private Period _period;
    private double _amount;
    private string _description;

    internal SpendingCategory(
        string userId,
        string name,
        Period period,
        Frequency frequency,
        double amount,
        string description
    ) : this(Guid.Empty, new List<Event>())
    {
        Id = Guid.NewGuid();

        AddChange(
            new SpendingCategoryCreated(
                Id,
                userId,
                name,
                period,
                frequency,
                amount,
                description
            )
        );
    }

    internal SpendingCategory(Guid id, IEnumerable<Event> changes) : base(id, changes)
    {
        InitializeMembers();
    }

    public string UserId 
    { 
        get
        {
            return _userId;
        }
        private set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("User id cannot be null or empty.");

            _userId = value;
        }
    }

    public string Name 
    { 
        get
        {
            return _name;
        }
        private set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Name cannot be null or empty.");

            _name = value;
        }
    }

    public Period Period 
    { 
        get
        {
            return _period;
        }
        private set
        {
            if (value is null)
                throw new ArgumentNullException("Period cannot be null.");
            
            _period = value;
        }   
    }

    public Frequency Frequency { get; private set; }

    public double Amount 
    { 
        get
        {
            return _amount;
        }
        private set
        {
            if (value < 0)
                throw new ArgumentException("Amount cannot be lower than 0.");

            _amount = value;
        }
    }

    public string Description 
    {
        get
        {
            return _description;
        }
        private set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Description cannot be null or empty.");

            _description = value;
        }
    }

    public void Update(string name, Frequency frequency, double amount, string description)
    {
        AddChange(
            new SpendingCategoryUpdated(
                Id,
                name,
                frequency,
                amount,
                description
            )
        );
    }

    public void ClosePeriod()
    {
        AddChange(
            new SpendingCategoryPeriodClosed(
                Id,
                DateTime.Now
            )
        );
    }

    public void OpenPeriod()
    {
        AddChange(
            new SpendingCategoryPeriodOpened(
                Id
            )
        );
    }

    [MemberNotNull(nameof(_userId))]
    [MemberNotNull(nameof(_name))]
    [MemberNotNull(nameof(_period))]
    [MemberNotNull(nameof(_description))]
    protected override void InitializeMembers()
    {
        _userId = string.Empty;
        _name = string.Empty;
        _period = new Period(DateTime.MinValue);
        _amount = 0;
        _description = string.Empty;
    }

    void IEventHandler<SpendingCategoryCreated>.Handle(SpendingCategoryCreated @event)
    {
        Id = @event.AggregateId;

        UserId = @event.UserId;
        Name = @event.Name;
        Period = @event.Period;
        Frequency = @event.Frequency;
        Amount = @event.Amount;
        Description = @event.Description;
    }

    void IEventHandler<SpendingCategoryUpdated>.Handle(SpendingCategoryUpdated @event)
    {
        Name = @event.Name;
        Frequency = @event.Frequency;
        Amount = @event.Amount;
        Description = @event.Description;
    }

    void IEventHandler<SpendingCategoryPeriodClosed>.Handle(SpendingCategoryPeriodClosed @event)
    {
        Period.Close(@event.EndDate);
    }

    void IEventHandler<SpendingCategoryPeriodOpened>.Handle(SpendingCategoryPeriodOpened @event)
    {
        Period.Open();
    }
}