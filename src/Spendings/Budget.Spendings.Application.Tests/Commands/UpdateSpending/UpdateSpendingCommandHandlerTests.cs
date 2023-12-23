using Budget.Spendings.Application.Commands.UpdateSpending;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Domain.Repositories;

using AutoFixture;
using FluentAssertions;
using NSubstitute;

using Microsoft.Extensions.Logging;
using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Application.Commands.CreateSpending;
using Budget.Spendings.Domain.Entities;

namespace Budget.Spendings.Application.Tests.Commands.UpdateSpending;

public class UpdateSpendingCommandHandlerTests
{
    private readonly Fixture _fixture;
    private readonly ISpendingRepository _spendingRepository;
    private readonly ISpendingCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSpendingHandler> _logger;
    private readonly UpdateSpendingHandler _handler;
    private readonly SpendingFactory _spendingFactory;
    private readonly SpendingCategoryFactory _categoryFactory;

    public UpdateSpendingCommandHandlerTests()
    {
        _fixture = new Fixture();

        _spendingRepository = Substitute.For<ISpendingRepository>();
        _categoryRepository = Substitute.For<ISpendingCategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _unitOfWork.SpendingCategories.Returns(_categoryRepository);
        _unitOfWork.Spendings.Returns(_spendingRepository);

        _logger = Substitute.For<ILogger<UpdateSpendingHandler>>();

        _handler = new UpdateSpendingHandler(_unitOfWork, _logger);

        _spendingFactory = new SpendingFactory();
        _categoryFactory = new SpendingCategoryFactory();
    }

    [Fact]
    public async Task Handle_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(null!, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_ThrowsSpendingDoesNotExistException_WhenSpendingDoesNotExist()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var command = new UpdateSpendingCommand(
            Guid.NewGuid(),
            _fixture.Create<string>(),
            Guid.NewGuid(),
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        //Act
        var action = (() => _handler.Handle(command!, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<SpendingDoesNotExistException>();
    }

    [Fact]
    public async void Handle_ThrowsSpendingBelongsToAnotherUserException_WhenSpendingsCategoryBelongsToAnotherUser()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var spending = _spendingFactory.Create(
            category.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);
        _spendingRepository.GetAsync(Arg.Is(spending.Id)).Returns(spending);

        var command = new UpdateSpendingCommand(
            spending.Id,
            _fixture.Create<string>(),
            Guid.NewGuid(),
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(command, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<SpendingBelongsToAnotherUserException>();
    }

    [Fact]
    public async Task Handle_ThrowsCategoryDoesNotExistException_WhenNewCategoryDoesNotExist()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var spending = _spendingFactory.Create(
            category.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);
        _spendingRepository.GetAsync(Arg.Is(spending.Id)).Returns(spending);

        var command = new UpdateSpendingCommand(
            spending.Id,
            category.UserId,
            Guid.NewGuid(),
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(command, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<CategoryDoesNotExistException>();

        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_ThrowsCategoryBelongsToAnotherUserException_WhenNewCategoryBelongsToAnotherUser()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var newCategory = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var spending = _spendingFactory.Create(
            category.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);
        _categoryRepository.GetAsync(Arg.Is(newCategory.Id)).Returns(newCategory);
        _spendingRepository.GetAsync(Arg.Is(spending.Id)).Returns(spending);

        var command = new UpdateSpendingCommand(
            spending.Id,
            category.UserId,
            newCategory.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(command, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<CategoryBelongsToAnotherUserException>();

        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_UpdatesSpending()
    {
        //Arrange
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var newCategory = _categoryFactory.Create(
            category.UserId,
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var spending = _spendingFactory.Create(
            category.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);
        _categoryRepository.GetAsync(Arg.Is(newCategory.Id)).Returns(newCategory);
        _spendingRepository.GetAsync(Arg.Is(spending.Id)).Returns(spending);

        var command = new UpdateSpendingCommand(
            spending.Id,
            category.UserId,
            newCategory.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var tokenSource = new CancellationTokenSource();

        //Act
        await _handler.Handle(command, tokenSource.Token);

        //Assert
        _unitOfWork.Received(1).BeginTransaction();
        
        await _unitOfWork.Spendings.Received(1).SaveAsync(
            Arg.Is<Spending>(s => 
                s.CategoryId == newCategory.Id
                && s.Date == command.Date
                && s.Amount == command.Amount
                && s.Description == command.Description
            )
        );

        _unitOfWork.Received(1).Commit();
    }
}