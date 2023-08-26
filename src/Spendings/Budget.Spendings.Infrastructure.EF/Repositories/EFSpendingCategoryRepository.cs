using Budget.Spendings.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Infrastructure.EF.Repositories;

public class EFSpendingCategoryRepository : ISpendingCategoryRepository
{
    private readonly SpendingsContext _context;

    public EFSpendingCategoryRepository(SpendingsContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.SpendingCategory?> GetAsync(Guid id)
    {
        SpendingCategory? dbSpendingCategory = await _context.SpendingCategories.AsNoTracking().Include(
            c => c.Events
        ).FirstOrDefaultAsync(
            c => c.Id == id
        );

        return dbSpendingCategory != null ? 
            dbSpendingCategory.ToDomain()
            : null;
    }

    
public async Task<IEnumerable<Domain.Entities.SpendingCategory>> GetAsync(string userId)
    {
        var categories = new List<Domain.Entities.SpendingCategory>();
        var dbCategories = await _context.SpendingCategories.AsNoTracking().Include(
            c => c.Events
        ).Where(
            c => c.UserId == userId
        ).ToListAsync();

        if (dbCategories != null)
            categories.AddRange(dbCategories.ConvertAll(c => c.ToDomain()));

        return categories;
    }
    

    public async Task<Domain.Entities.SpendingCategory?> GetAsync(string userId, string name)
    {
        SpendingCategory? dbSpendingCategory = await _context.SpendingCategories.AsNoTracking().Include(
            c => c.Events
        ).FirstOrDefaultAsync(
            c => c.UserId == userId && c.Name == name
        );

        return dbSpendingCategory != null ? 
            dbSpendingCategory.ToDomain()
            : null;
    }

    public async Task SaveAsync(Domain.Entities.SpendingCategory category)
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