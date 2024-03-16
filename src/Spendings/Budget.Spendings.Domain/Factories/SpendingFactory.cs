using Budget.EventSourcing.Events;
using Budget.Spendings.Domain.Entities;

namespace Budget.Spendings.Domain.Factories;

public class SpendingFactory
{
    public SpendingFactory()
    {
    }

    public Spending Create(
        Guid categoryId,
        DateTime date,
        double amount,
        string description
    )
    {
        return new Spending(
            categoryId,
            date,
            amount,
            description
        );
    }

    public Spending Load(Guid id, IEnumerable<Event> changes)
    {
        Spending spending = new Spending(id, changes);
        spending.ApplyChangeHistory();

        return spending;
    }
}