using Budget.Spendings.Domain.ReadModel.Entities;

namespace Budget.Spendings.Domain.ReadModel.Repositories;

public interface ISpendingCategoryRepository
{
    Task<SpendingCategory?> Get(Guid id);

    Task<IEnumerable<SpendingCategory>> Get(string userId);

    Task<SpendingCategory?> Get(string userId, string name);
    
}