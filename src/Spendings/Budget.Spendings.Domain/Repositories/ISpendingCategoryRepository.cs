using Budget.Spendings.Domain.Entities;

namespace Budget.Spendings.Domain.Repositories;

public interface ISpendingCategoryRepository
{
    Task<SpendingCategory?> GetAsync(Guid id);

    Task<SpendingCategory?> GetAsync(string userId, string name);

    Task<IEnumerable<SpendingCategory>> GetAsync(string userId);

    Task SaveAsync(SpendingCategory category);

    Task DeleteAsync(Guid id);
}