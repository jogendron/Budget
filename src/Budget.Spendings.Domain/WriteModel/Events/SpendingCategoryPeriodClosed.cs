using Budget.EventSourcing.Events;

namespace Budget.Spendings.Domain.WriteModel.Events;

public class SpendingCategoryPeriodClosed : Event
{
    public SpendingCategoryPeriodClosed() : base()
    {
        EndDate = DateTime.MinValue;
    }

    public SpendingCategoryPeriodClosed(Guid aggregateId, DateTime endDate) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
    {
        EndDate = endDate;
    }

    public DateTime EndDate { get; set; }
}
