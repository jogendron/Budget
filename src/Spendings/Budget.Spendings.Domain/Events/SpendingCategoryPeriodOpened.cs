using Budget.EventSourcing.Events;

namespace Budget.Spendings.Domain.Events;

public class SpendingCategoryPeriodOpened : Event
{
    public SpendingCategoryPeriodOpened() : base()
    {
    }

    public SpendingCategoryPeriodOpened(Guid aggregateId) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
    {
    }
}