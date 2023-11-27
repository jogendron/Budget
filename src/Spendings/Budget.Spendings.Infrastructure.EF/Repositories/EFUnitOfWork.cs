using Budget.Spendings.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Infrastructure.EF.Repositories;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly SpendingsContext _dbContext;

    public EFUnitOfWork(SpendingsContext dbContext)
    {
        _dbContext = dbContext;
        SpendingCategories = new EFSpendingCategoryRepository(_dbContext);
        Spendings = new EFSpendingRepository(_dbContext);
    }

    public ISpendingCategoryRepository SpendingCategories { get; }

    public ISpendingRepository Spendings { get; }

    public void BeginTransaction()
    {
        if (_dbContext.Database.IsRelational())
            _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        if (_dbContext.Database.IsRelational())
            _dbContext.Database.CommitTransaction();
    }

    public void Rollback()
    {
        if (_dbContext.Database.IsRelational())
            _dbContext.Database.RollbackTransaction();
    }
}