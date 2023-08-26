using Budget.Spendings.Domain.Repositories;

namespace Budget.Spendings.Infrastructure.EF.Repositories;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly SpendingsContext _dbContext;

    public EFUnitOfWork(SpendingsContext dbContext)
    {
        _dbContext = dbContext;
        SpendingCategories = new EFSpendingCategoryRepository(_dbContext);
    }

    public ISpendingCategoryRepository SpendingCategories { get; }

    public void BeginTransaction()
    {
        _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        _dbContext.Database.CommitTransaction();
    }

    public void Rollback()
    {
        _dbContext.Database.RollbackTransaction();
    }
}