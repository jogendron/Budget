using Budget.Spendings.Infrastructure.EF.ReadModel.Repositories;

using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Infrastructure.EF.Tests.Domain.ReadModel.Repositories;

public class EFReadModelSpendingCategoryRepositoryTests
{
    private readonly Fixture _fixture;
    private readonly SpendingsContext _dbContext;
    private readonly EFReadModelSpendingCategoryRepository _repository;

    public EFReadModelSpendingCategoryRepositoryTests()
    {
        _fixture = new Fixture();

        var builder = new DbContextOptionsBuilder<SpendingsContext>();
        builder.UseInMemoryDatabase(_fixture.Create<string>());
        _dbContext = new SpendingsContext(builder.Options);
        _dbContext.Database.EnsureCreated();

        _repository = new EFReadModelSpendingCategoryRepository(_dbContext);
    }

    private void AssertEqual(SpendingCategory dbCategory, Spendings.Domain.ReadModel.Entities.SpendingCategory category)
    {
        category.Id.Should().Be(dbCategory.Id);
        category.UserId.Should().Be(dbCategory.UserId);
        category.Name.Should().Be(dbCategory.Name);
        category.CreatedOn.Should().Be(dbCategory.BeginDate);
        category.ModifiedOn.Should().Be(dbCategory.ModifiedOn);
        category.ClosedOn.Should().Be(dbCategory.EndDate);
        category.Frequency.Should().Be(dbCategory.Frequency.ToReadModel());
        category.Amount.Should().Be(dbCategory.Amount);
        category.Description.Should().Be(dbCategory.Description);
    }

    [Fact]
    public async Task GetById_ReturnsCategory_WhenFound()
    {
        //Arrange
        var dbCategory = _fixture.Create<SpendingCategory>();
        _dbContext.SpendingCategories.Add(dbCategory);
        _dbContext.SaveChanges();

        //Act
        var category = await _repository.Get(dbCategory.Id);

        //Assert
        category.Should().NotBeNull();
        
        if (category != null)
            AssertEqual(dbCategory, category);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenNotFound()
    {
        //Arrange
        var dbCategory = _fixture.Create<SpendingCategory>();
        _dbContext.SpendingCategories.Add(dbCategory);
        _dbContext.SaveChanges();

        //Act
        var category = await _repository.Get(Guid.NewGuid());

        //Asssert
        category.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserId_ReturnsAllMatchingItems_WhenFound()
    {
        //Arrange
        var dbCategory1 = _fixture.Create<SpendingCategory>();
        var dbCategory2 = _fixture.Create<SpendingCategory>();
        var dbCategory3 = _fixture.Create<SpendingCategory>();

        var userId = _fixture.Create<string>();
        dbCategory1.UserId = userId;
        dbCategory2.UserId = userId;

        _dbContext.SpendingCategories.Add(dbCategory1);
        _dbContext.SpendingCategories.Add(dbCategory2);
        _dbContext.SpendingCategories.Add(dbCategory3);
        _dbContext.SaveChanges();

        //Act
        var categories = await _repository.Get(userId);

        //Assert
        categories.Should().NotBeNullOrEmpty();

        if (categories != null)
        {
            categories.Count().Should().Be(2);
            var category1 = categories.First();
            var category2 = categories.Skip(1).First();
            
            category1.Should().NotBeNull();
            category2.Should().NotBeNull();

            if (category1 != null)
                AssertEqual(dbCategory1, category1);

            if (category2 != null)
                AssertEqual(dbCategory2, category2);
        }
    }

    [Fact]
    public async Task GetByUserId_ReturnsEmptyList_WhenNoneFound()
    {
        //Arrange
        var dbCategory1 = _fixture.Create<SpendingCategory>();
        var dbCategory2 = _fixture.Create<SpendingCategory>();
        var dbCategory3 = _fixture.Create<SpendingCategory>();

        var userId = _fixture.Create<string>();

        _dbContext.SpendingCategories.Add(dbCategory1);
        _dbContext.SpendingCategories.Add(dbCategory2);
        _dbContext.SpendingCategories.Add(dbCategory3);
        _dbContext.SaveChanges();

        //Act
        var categories = await _repository.Get(userId);

        //Assert
        categories.Should().NotBeNull();
        categories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserIdAndName_ReturnsCategory_WhenFound()
    {
        //Arrange
        var dbCategory = _fixture.Create<SpendingCategory>();
        _dbContext.SpendingCategories.Add(dbCategory);
        _dbContext.SaveChanges();

        //Act
        var category = await _repository.Get(dbCategory.UserId, dbCategory.Name);

        //Assert
        category.Should().NotBeNull();
        
        if (category != null)
            AssertEqual(dbCategory, category);
    }

    [Fact]
    public async Task GetByUserIdAndName_ReturnsNull_WhenNotFound()
    {
        //Arrange
        var dbCategory = _fixture.Create<SpendingCategory>();
        _dbContext.SpendingCategories.Add(dbCategory);
        _dbContext.SaveChanges();

        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();

        //Act
        var category = await _repository.Get(userId, name);

        //Assert
        category.Should().BeNull();
    }
}