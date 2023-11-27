namespace Budget.Spendings.Domain.Repositories;

public interface IUnitOfWork
{
    ISpendingCategoryRepository SpendingCategories { get; }

    ISpendingRepository Spendings { get; }

    void BeginTransaction();

    void Commit();

    void Rollback();
    
}