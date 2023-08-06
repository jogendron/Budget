using Budget.Spendings.Domain.WriteModel.Factories;
using Budget.Spendings.Domain.WriteModel.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Infrastructure.EF.WriteModel.Repositories;

public class EFWriteModelSpendingCategoryRepository : ISpendingCategoryRepository
{
    private readonly SpendingsContext _context;
    private readonly SpendingCategoryFactory _factory;

    public EFWriteModelSpendingCategoryRepository(SpendingsContext context)
    {
        _context = context;
        _factory = new SpendingCategoryFactory();
    }

    public async Task<Domain.WriteModel.Entities.SpendingCategory?> GetAsync(Guid id)
    {
        SpendingCategory? dbSpendingCategory = await _context.SpendingCategories.AsNoTracking().Include(
            c => c.Events
        ).FirstOrDefaultAsync(
            c => c.Id == id
        );

        return dbSpendingCategory != null ? 
            _factory.Load(id, dbSpendingCategory.Events.Select(e => e.ToDomainEvent())) 
            : null;
    }

    public async Task<Domain.WriteModel.Entities.SpendingCategory?> GetAsync(string userId, string name)
    {
        SpendingCategory? dbSpendingCategory = await _context.SpendingCategories.AsNoTracking().Include(
            c => c.Events
        ).FirstOrDefaultAsync(
            c => c.UserId == userId && c.Name == name
        );

        return dbSpendingCategory != null ? 
            _factory.Load(dbSpendingCategory.Id, dbSpendingCategory.Events.Select(e => e.ToDomainEvent())) 
            : null;
    }

    public async Task SaveAsync(Domain.WriteModel.Entities.SpendingCategory category)
    {
        var dbCategory = new SpendingCategory(category);
        var existingCategory = await _context.SpendingCategories.FindAsync(category.Id);

        if (existingCategory != null)
        {
            var entry = _context.SpendingCategories.Entry(existingCategory);
            entry.CurrentValues.SetValues(dbCategory);
        }
        else
        {
            _context.SpendingCategories.Add(dbCategory);
        }

        _context.SaveChanges();
    }
}