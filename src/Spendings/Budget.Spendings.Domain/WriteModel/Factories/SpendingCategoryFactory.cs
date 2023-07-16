using Budget.EventSourcing.Events;
using Budget.Spendings.Domain.WriteModel.Entities;

namespace Budget.Spendings.Domain.WriteModel.Factories;

public class SpendingCategoryFactory
{
    public SpendingCategory Create(
        string userId,
        string name,
        Frequency frequency,
        double amount,
        string description
    )
    {
        return new SpendingCategory(
            userId,
            name,
            new Period(DateTime.Now),
            frequency,
            amount,
            description
        );
    }

    public SpendingCategory Load(Guid id, IEnumerable<Event> changes)
    {
        SpendingCategory category = new SpendingCategory(id, changes);
        category.ApplyChangeHistory();

        return category;
    }
}