using Budget.Spendings.Domain.WriteModel.Repositories;

namespace Budget.Spendings.Infrastructure.EF.WriteModel.Repositories;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly SpendingsContext _dbContext;

    public EFUnitOfWork(SpendingsContext dbContext)
    {
        _dbContext = dbContext;
        SpendingCategories = new EFWriteModelSpendingCategoryRepository(_dbContext);
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