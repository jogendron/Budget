using Budget.Spendings.Domain.ReadModel.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Infrastructure.EF.ReadModel.Repositories;

public class EFReadModelSpendingCategoryRepository : ISpendingCategoryRepository
{
    private readonly SpendingsContext _dbContext;

    public EFReadModelSpendingCategoryRepository(SpendingsContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Domain.ReadModel.Entities.SpendingCategory?> Get(Guid id)
    {
        Domain.ReadModel.Entities.SpendingCategory? category = null;
        var dbCategory = await _dbContext.SpendingCategories.AsNoTracking().FirstOrDefaultAsync(
            c => c.Id == id
        );

        if (dbCategory != null)
            category = dbCategory.ToReadModel();

        return category;
    }

    public async Task<IEnumerable<Domain.ReadModel.Entities.SpendingCategory>> Get(string userId)
    {
        var categories = new List<Domain.ReadModel.Entities.SpendingCategory>();
        var dbCategories = await _dbContext.SpendingCategories.AsNoTracking().Where(
            c => c.UserId == userId
        ).ToListAsync();

        if (dbCategories != null)
            categories.AddRange(dbCategories.ConvertAll(c => c.ToReadModel()));

        return categories;
    }

    public async Task<Domain.ReadModel.Entities.SpendingCategory?> Get(string userId, string name)
    {
        Domain.ReadModel.Entities.SpendingCategory? category = null;
        var dbCategory = await _dbContext.SpendingCategories.AsNoTracking().FirstOrDefaultAsync(
            c => c.UserId == userId
            && c.Name == name
        );

        if (dbCategory != null)
            category = dbCategory.ToReadModel();

        return category;
    }
}