using Budget.Spendings.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Infrastructure.EF.Repositories;

public class EFSpendingRepository : ISpendingRepository
{
    private readonly SpendingsContext _context;

    public EFSpendingRepository(SpendingsContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Spending?> GetAsync(Guid id)
    {
        Spending? dbSpending = await _context.Spendings.AsNoTracking().Include(
            c => c.Events
        ).FirstOrDefaultAsync(
            c => c.Id == id
        );

        return dbSpending != null ? 
            dbSpending.ToDomain()
            : null;
    }

    public async Task<IEnumerable<Domain.Entities.Spending>> GetAsync(Guid categoryId, DateTime? beginDate, DateTime? endDate)
    {
        var spendings = new List<Domain.Entities.Spending>();
        DateTime begin = beginDate ?? DateTime.MinValue;
        DateTime end = endDate ?? DateTime.MaxValue;

        var dbSpendings = await _context.Spendings.Include(s => s.Events).Where(
            s => s.SpendingCategoryId == categoryId
                && s.Date >= begin 
                && s.Date <= end
        ).ToListAsync();

        spendings.AddRange(dbSpendings.Select(d => d.ToDomain()));

        return spendings;
    }

    public async Task<IEnumerable<Domain.Entities.Spending>> GetAsync(string userId, DateTime? beginDate, DateTime? endDate)
    {
        var spendings = new List<Domain.Entities.Spending>();
        DateTime begin = beginDate ?? DateTime.MinValue;
        DateTime end = endDate ?? DateTime.MaxValue;

        var categoryIds = await _context.SpendingCategories.AsNoTracking().Where(
            s => s.UserId == userId
        ).Select(s => s.Id).ToListAsync();

        var dbSpendings = await _context.Spendings.Include(s => s.Events).Where(
            s => categoryIds.Contains(s.SpendingCategoryId)
                && s.Date >= begin 
                && s.Date <= end
        ).ToListAsync();

        spendings.AddRange(dbSpendings.Select(d => d.ToDomain()));

        return spendings;
    }

    public async Task SaveAsync(Domain.Entities.Spending spending)
    {
        var dbSpending = new Spending(spending);
        var existingSpending = await _context.Spendings.Include(
            s => s.Events
        ).FirstOrDefaultAsync(c => c.Id == spending.Id);

        if (existingSpending != null)
        {
            _context.Spendings.Entry(
                existingSpending
            ).CurrentValues.SetValues(dbSpending);
            
            existingSpending.Events.AddRange(
                spending.NewChanges.Where(
                    c => ! existingSpending.Events.Any(e => e.Id == c.EventId)
                ).Select(
                    c => new Event(c)
                )
            );
        }
        else
        {
            _context.Spendings.Add(dbSpending);
        }

        _context.SaveChanges();
    }
}