using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Infrastructure.EF.Repositories;

using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Budget.Spendings.Domain.Events;

namespace Budget.Spendings.Infrastructure.EF.Tests.Repositories;

public class EFSpendingCategoryRepositoryTests
{
    private readonly Fixture _fixture;
    private readonly SpendingsContext _dbContext;
    private readonly EFSpendingCategoryRepository _repository;
    private SpendingCategoryFactory _categoryFactory;

    public EFSpendingCategoryRepositoryTests()
    {
        _fixture = new Fixture();

        var builder = new DbContextOptionsBuilder<SpendingsContext>();
        builder.UseInMemoryDatabase(_fixture.Create<string>());
        _dbContext = new SpendingsContext(builder.Options);
        _dbContext.Database.EnsureCreated();

        _repository = new EFSpendingCategoryRepository(_dbContext);

        _categoryFactory = new SpendingCategoryFactory();
    }

    private SpendingCategory CreateDbCategory(string userId)
    {
        var id = Guid.NewGuid();

        var @event = _fixture.Build<SpendingCategoryCreated>().With(
            e => e.AggregateId, id
        ).With(
            e => e.UserId, userId
        ).With(
            e => e.Period, new Domain.Events.Period(DateTime.MinValue, DateTime.MaxValue)
        ).Create();

        return _fixture.Build<SpendingCategory>().With(
            c => c.Id, id
        ).With(
            c => c.UserId, @event.UserId
        ).With(
            c => c.Name, @event.Name
        ).With(
            c => c.BeginDate, @event.Period.BeginDate
        ).With(
            c => c.EndDate, @event.Period.EndDate
        ).With(
            c => c.Frequency, @event.Frequency.ToDomainFrequency().ToDbFrequency()
        ).With(
            c => c.Amount, @event.Amount
        ).With(
            c => c.Description, @event.Description
        ).With(
            c => c.Events, new List<Event>() {
                new Event(@event)
            }
        ).Create();
    }

    private void AssertEqual(SpendingCategory dbCategory, Domain.Entities.SpendingCategory category)
    {
        category.Id.Should().Be(dbCategory.Id);
        category.UserId.Should().Be(dbCategory.UserId);
        category.Name.Should().Be(dbCategory.Name);
        category.Period.BeginDate.Should().Be(dbCategory.BeginDate);
        category.Period.EndDate.Should().Be(dbCategory.EndDate);
        category.Frequency.Should().Be(dbCategory.Frequency.ToDomainFrequency());
        category.Amount.Should().Be(dbCategory.Amount);
        category.Description.Should().Be(dbCategory.Description);
    }


    [Fact]
    public async Task GetByIdAsync_LoadsAndReturns_Category()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Budget.Spendings.Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        await _repository.SaveAsync(category);

        //Act
        var retreivedCategory = await _repository.GetAsync(category.Id);

        //Assert
        retreivedCategory.Should().NotBeNull();

        if (retreivedCategory != null)
        {
            retreivedCategory.Id.Should().Be(category.Id);
            retreivedCategory.UserId.Should().Be(category.UserId);
            retreivedCategory.Name.Should().Be(category.Name);
            retreivedCategory.Frequency.Should().Be(category.Frequency);
            retreivedCategory.Amount.Should().Be(category.Amount);
            retreivedCategory.Description.Should().Be(category.Description);
            retreivedCategory.Changes.Should().BeEquivalentTo(category.Changes);
        }        
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenCategoryIsNotFound()
    {
        //Arrange
        Guid id = Guid.NewGuid();

        //Act
        var retreivedCategory = await _repository.GetAsync(id);

        //Assert
        retreivedCategory.Should().BeNull();  
    }

    [Fact]
    public async Task GetByUserId_ReturnsAllMatchingItems_WhenFound()
    {
        //Arrange
        var userId1 = _fixture.Create<string>();
        var userId2 = _fixture.Create<string>();

        var dbCategory1 = CreateDbCategory(userId1);
        var dbCategory2 = CreateDbCategory(userId1);
        var dbCategory3 = CreateDbCategory(userId2);

        _dbContext.SpendingCategories.Add(dbCategory1);
        _dbContext.SpendingCategories.Add(dbCategory2);
        _dbContext.SpendingCategories.Add(dbCategory3);
        _dbContext.SaveChanges();

        //Act
        var categories = await _repository.GetAsync(userId1);

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
        var categories = await _repository.GetAsync(userId);

        //Assert
        categories.Should().NotBeNull();
        categories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserIdAndNameAsync_LoadsAndReturns_Category()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Budget.Spendings.Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        await _repository.SaveAsync(category);

        //Act
        var retreivedCategory = await _repository.GetAsync(category.UserId, category.Name);

        //Assert
        retreivedCategory.Should().NotBeNull();

        if (retreivedCategory != null)
        {
            retreivedCategory.Id.Should().Be(category.Id);
            retreivedCategory.UserId.Should().Be(category.UserId);
            retreivedCategory.Name.Should().Be(category.Name);
            retreivedCategory.Frequency.Should().Be(category.Frequency);
            retreivedCategory.Amount.Should().Be(category.Amount);
            retreivedCategory.Description.Should().Be(category.Description);
            retreivedCategory.Changes.Should().BeEquivalentTo(category.Changes);
        }        
    }

    [Fact]
    public async Task GetByUserIdAndNameAsync_ReturnsNull_WhenCategoryIsNotFound()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();

        //Act
        var retreivedCategory = await _repository.GetAsync(userId, name);

        //Assert
        retreivedCategory.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_InsertsCategory_WhenNotFound()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            Budget.Spendings.Domain.Entities.Frequency.SemiWeekly,
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        //Act
        await _repository.SaveAsync(category);

        //Assert
        _dbContext.SpendingCategories.Count().Should().Be(1);
        _dbContext.SpendingCategories.Should().Contain(c => c.Id == category.Id);
   
        var dbCategory = _dbContext.SpendingCategories.Include(
            c => c.Events
        ).First(c => c.Id == category.Id);

        dbCategory.UserId.Should().Be(category.UserId);
        dbCategory.Name.Should().Be(category.Name);
        dbCategory.BeginDate.Should().Be(category.Period.BeginDate);
        dbCategory.ModifiedOn.Should().Be(category.Changes.Max(c => c.EventDate));
        dbCategory.EndDate.Should().Be(category.Period.EndDate);
        dbCategory.Frequency.Should().Be(Frequency.SemiWeekly);
        dbCategory.Amount.Should().Be(category.Amount);
        dbCategory.Description.Should().Be(category.Description);
        dbCategory.Events.Count().Should().Be(category.Changes.Count());
    }

    [Fact]
    public async Task SaveAsync_UpdatesCategory_WhenFound()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        await _repository.SaveAsync(category);
        category = _categoryFactory.Load(category.Id, category.Changes);

        var newName = _fixture.Create<string>();
        category.Update(
            newName,
            category.Frequency,
            category.Amount,
            category.Description
        );

        //Act
        await _repository.SaveAsync(category);

        //Assert
        _dbContext.SpendingCategories.Count().Should().Be(1);
        _dbContext.SpendingCategories.Should().Contain(c => c.Id == category.Id);
        _dbContext.SpendingCategories.First(c => c.Id == category.Id).Events.Count().Should().Be(2);
        _dbContext.SpendingCategories.First(c => c.Id == category.Id).Name.Should().Be(newName);
    }
    
    [Fact]
    public async Task DeleteAsync_RemovesCategory()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        await _repository.SaveAsync(category);

        //Act
        await _repository.DeleteAsync(category.Id);

        //Assert
        var search = await _repository.GetAsync(category.Id);
        search.Should().BeNull();
    }
}