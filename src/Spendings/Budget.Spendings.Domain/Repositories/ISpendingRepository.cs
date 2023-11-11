using Budget.Spendings.Domain.Entities;

namespace Budget.Spendings.Domain.Repositories;

public interface ISpendingRepository
{
    Task<Spending?> GetAsync(Guid id);

    Task<IEnumerable<Spending>> GetAsync(Guid categoryId, DateTime? beginDate, DateTime? endDate);

    Task<IEnumerable<Spending>> GetAsync(string userId, DateTime? beginDate, DateTime? endDate);

    Task SaveAsync(Spending spending);
}