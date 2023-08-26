namespace Budget.Spendings.Domain.Repositories;

public interface IUnitOfWork
{
    ISpendingCategoryRepository SpendingCategories { get; }

    void BeginTransaction();

    void Commit();

    void Rollback();
    
}