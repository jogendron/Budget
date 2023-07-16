namespace Budget.Spendings.Domain.WriteModel.Repositories;

public interface IUnitOfWork
{
    ISpendingCategoryRepository SpendingCategories { get; }

    void BeginTransaction();

    void Commit();

    void Rollback();
    
}