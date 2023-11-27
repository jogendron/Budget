using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Infrastructure.EF.Repositories;

using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Budget.Spendings.Domain.Events;

namespace Budget.Spendings.Infrastructure.EF.Tests.Repositories;

public class EFSpendingRepositoryTests
{
    private readonly Fixture _fixture;
    private readonly SpendingsContext _dbContext;
    private readonly EFSpendingCategoryRepository _spendingCategoryRepository;
    private readonly EFSpendingRepository _spendingRepository;
    private readonly SpendingCategoryFactory _spendingCategoryFactory;
    private readonly SpendingFactory _spendingFactory;

    public EFSpendingRepositoryTests()
    {
        _fixture = new Fixture();

        var builder = new DbContextOptionsBuilder<SpendingsContext>();
        builder.UseInMemoryDatabase(_fixture.Create<string>());
        _dbContext = new SpendingsContext(builder.Options);
        _dbContext.Database.EnsureCreated();

        _spendingCategoryRepository = new EFSpendingCategoryRepository(_dbContext);
        _spendingRepository = new EFSpendingRepository(_dbContext);

        _spendingCategoryFactory = new SpendingCategoryFactory();
        _spendingFactory = new SpendingFactory();
    }

    [Fact]
    public async Task GetAsyncById_LoadsAndReturns_Spending()
    {
        //Arrange
        var spending = _spendingFactory.Create(
            Guid.NewGuid(),
            _fixture.Create<DateTime>(),
            (new Random()).NextDouble() * 1000,
            _fixture.Create<string>()
        );

        await _spendingRepository.SaveAsync(spending);

        //Act
        var retrievedSpending = await _spendingRepository.GetAsync(spending.Id);

        //Assert
        retrievedSpending.Should().NotBeNull();

        if (retrievedSpending != null)
        {
            retrievedSpending.Id.Should().Be(spending.Id);
            retrievedSpending.CategoryId.Should().Be(spending.CategoryId);
            retrievedSpending.Date.Should().Be(spending.Date);
            retrievedSpending.Amount.Should().Be(spending.Amount);
            retrievedSpending.Description.Should().Be(spending.Description);
            retrievedSpending.Changes.Should().BeEquivalentTo(spending.Changes);
        }
    }

    [Fact]
    public async Task GetAsyncById_ReturnsNull_WhenSpendingDoesNotExist()
    {
        //Arrange

        //Act
        var spending = await _spendingRepository.GetAsync(Guid.NewGuid());

        //Assert
        spending.Should().BeNull();
    }

    [Fact]
    public async Task GetAsyncByCategory_LoadsAndReturns_Spendings()
    {
        //Arrange
        var categoryId = Guid.NewGuid();

        var spending1 = _spendingFactory.Create(
            categoryId,
            DateTime.MinValue,
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var spending2 = _spendingFactory.Create(
            categoryId,
            DateTime.MinValue.AddDays(1),
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var spending3 = _spendingFactory.Create(
            categoryId,
            DateTime.MaxValue.AddDays(-1),
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var spending4 = _spendingFactory.Create(
            categoryId,
            DateTime.MaxValue,
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        await _spendingRepository.SaveAsync(spending1);
        await _spendingRepository.SaveAsync(spending2);
        await _spendingRepository.SaveAsync(spending3);
        await _spendingRepository.SaveAsync(spending4);

        //Act
        var spendings = await _spendingRepository.GetAsync(
            categoryId, 
            DateTime.MinValue.AddDays(1), 
            DateTime.MaxValue.AddDays(-1)
        );

        //Assert
        spendings.Should().NotBeNull();
        spendings.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetAsyncByCategory_ReturnsEmptyCollection_WhenCategoryDoesNotExist()
    {
        //Arrange

        //Act
        var spendings = await _spendingRepository.GetAsync(
            Guid.NewGuid(),
            null,
            null
        );

        //Assert
        spendings.Should().NotBeNull();
        spendings.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetAsyncByUser_LoadsAndReturns_Spendings()
    {
        //Arrange
        var category = _spendingCategoryFactory.Create( 
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var spending1 = _spendingFactory.Create(
            category.Id,
            DateTime.MinValue,
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var spending2 = _spendingFactory.Create(
            category.Id,
            DateTime.MinValue.AddDays(1),
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var spending3 = _spendingFactory.Create(
            category.Id,
            DateTime.MaxValue.AddDays(-1),
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var spending4 = _spendingFactory.Create(
            category.Id,
            DateTime.MaxValue,
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        await _spendingCategoryRepository.SaveAsync(category);
        await Task.WhenAll(new [] {
            _spendingRepository.SaveAsync(spending1),
            _spendingRepository.SaveAsync(spending2),
            _spendingRepository.SaveAsync(spending3),
            _spendingRepository.SaveAsync(spending4)
        });

        //Act
        var spendings = await _spendingRepository.GetAsync(
            category.UserId, 
            DateTime.MinValue.AddDays(1), 
            DateTime.MaxValue.AddDays(-1)
        );

        //Assert
        spendings.Should().NotBeNull();
        spendings.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetAsyncByUser_ReturnsEmptyCollection_WhenUserDoesNotExist()
    {
        //Arrange

        //Act
        var spendings = await _spendingRepository.GetAsync( 
            _fixture.Create<string>(),
            DateTime.MinValue,
            DateTime.MaxValue
        );

        //Assert
        spendings.Should().NotBeNull();
        spendings.Count().Should().Be(0);
    }

    [Fact]
    public async Task SaveAsync_InsertSpending_WhenSpendingDoesNotExist()
    {
        //Arrange
        var spending = _spendingFactory.Create(
            Guid.NewGuid(),
            _fixture.Create<DateTime>(),
            (new Random()).NextDouble() * 1000,
            _fixture.Create<string>()
        );

        //Act
        await _spendingRepository.SaveAsync(spending);

        //Assert
        var dbSpending = await _dbContext.Spendings.Include(
            s => s.Events
        ).FirstOrDefaultAsync(s => s.Id == spending.Id);

        dbSpending.Should().NotBeNull();
        if (dbSpending != null)
        {
            dbSpending.Id.Should().Be(spending.Id);
            dbSpending.SpendingCategoryId.Should().Be(spending.CategoryId);
            dbSpending.Date.Should().Be(spending.Date);
            dbSpending.Amount.Should().Be(spending.Amount);
            dbSpending.Description.Should().Be(spending.Description);
        }
    }

    [Fact]
    public async Task SaveAsync_UpdatesSpending_WhenSpendingAlreadyExist()
    {
        //Arrange
        var spending = _spendingFactory.Create(
            Guid.NewGuid(),
            _fixture.Create<DateTime>(),
            new Random().NextDouble() * 1000,
            _fixture.Create<string>()
        );

        await _spendingRepository.SaveAsync(spending);

        spending.Update(
            Guid.NewGuid(),
            _fixture.Create<DateTime>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        //Act
        await _spendingRepository.SaveAsync(spending);
        
        //Assert
        _dbContext.Spendings.Count().Should().Be(1);

        var dbSpending = await _dbContext.Spendings.Include(
            s => s.Events
        ).FirstOrDefaultAsync(s => s.Id == spending.Id);

        dbSpending.Should().NotBeNull();
        if (dbSpending != null)
        {
            dbSpending.Id.Should().Be(spending.Id);
            dbSpending.SpendingCategoryId.Should().Be(spending.CategoryId);
            dbSpending.Date.Should().Be(spending.Date);
            dbSpending.Amount.Should().Be(spending.Amount);
            dbSpending.Description.Should().Be(spending.Description);
        }
    }
}