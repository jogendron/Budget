using Budget.Spendings.Domain.WriteModel.Factories;
using Budget.Spendings.Infrastructure.EF.WriteModel.Repositories;

using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Infrastructure.EF.Tests.Domain.WriteModel.Repositories;

public class EFWriteModelSpendingCategoryRepositoryTests
{
    private readonly Fixture _fixture;
    private readonly SpendingsContext _dbContext;
    private readonly EFWriteModelSpendingCategoryRepository _repository;
    private SpendingCategoryFactory _categoryFactory;

    public EFWriteModelSpendingCategoryRepositoryTests()
    {
        _fixture = new Fixture();

        var builder = new DbContextOptionsBuilder<SpendingsContext>();
        builder.UseInMemoryDatabase(_fixture.Create<string>());
        _dbContext = new SpendingsContext(builder.Options);
        _dbContext.Database.EnsureCreated();

        _repository = new EFWriteModelSpendingCategoryRepository(_dbContext);

        _categoryFactory = new SpendingCategoryFactory();
    }

    [Fact]
    public async Task GetByIdAsync_LoadsAndReturns_Category()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Budget.Spendings.Domain.WriteModel.Entities.Frequency>(),
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
    public async Task GetByUserIdAndNameAsync_LoadsAndReturns_Category()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Budget.Spendings.Domain.WriteModel.Entities.Frequency>(),
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
            Budget.Spendings.Domain.WriteModel.Entities.Frequency.SemiWeekly,
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
            _fixture.Create<Budget.Spendings.Domain.WriteModel.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        await _repository.SaveAsync(category);

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
        _dbContext.SpendingCategories.First(c => c.Id == category.Id).Name.Should().Be(newName);
    }
    
}