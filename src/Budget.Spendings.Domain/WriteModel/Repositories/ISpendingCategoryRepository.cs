using Budget.Spendings.Domain.WriteModel.Entities;

namespace Budget.Spendings.Domain.Repositories;

public interface ISpendingCategoryRepository
{
    Task<SpendingCategory?> GetAsync(Guid id);

    Task<SpendingCategory?> GetAsync(string userId, string name);

    Task SaveAsync(SpendingCategory category);
}