using Budget.EventSourcing.Events;

namespace Budget.Spendings.Domain.WriteModel.Events;

public class SpendingCategoryPeriodOpened : Event
{
    public SpendingCategoryPeriodOpened() : base()
    {
    }

    public SpendingCategoryPeriodOpened(Guid aggregateId) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
    {
    }
}