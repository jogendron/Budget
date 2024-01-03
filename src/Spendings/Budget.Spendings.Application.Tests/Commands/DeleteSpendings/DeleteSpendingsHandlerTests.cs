using Budget.Spendings.Application.Commands.DeleteSpendings;
using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Domain.Repositories;

using Microsoft.Extensions.Logging;

using AutoFixture;
using FluentAssertions;
using NSubstitute;

namespace Budget.Spendings.Application.Tests.Commands.DeleteSpendings;

public class DeleteSpendingsHandlerTests
{
    private readonly Fixture _fixture;
    private readonly ISpendingCategoryRepository _categoryRepository;
    private readonly ISpendingRepository _spendingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSpendingsHandler> _logger;
    private readonly DeleteSpendingsHandler _handler;
    private readonly SpendingCategoryFactory _categoryFactory;
    private readonly SpendingFactory _spendingFactory;

    public DeleteSpendingsHandlerTests()
    {
        _fixture = new Fixture();

        _categoryRepository = Substitute.For<ISpendingCategoryRepository>();
        _spendingRepository = Substitute.For<ISpendingRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.SpendingCategories.Returns(_categoryRepository);
        _unitOfWork.Spendings.Returns(_spendingRepository);

        _logger = Substitute.For<ILogger<DeleteSpendingsHandler>>();

        _handler = new DeleteSpendingsHandler(_unitOfWork, _logger);

        _categoryFactory = new SpendingCategoryFactory();
        _spendingFactory = new SpendingFactory();
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
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_ThrowsSpendingDoesNotExistException_WhenAtLeastOneSpendingIsNotFound()
    {
        //Arrange
        var spendings = new List<Spending>();
        var userId = _fixture.Create<string>();
        
        var category = _categoryFactory.Create(
            userId,
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);

        for (int i = 0; i < 3; i++)
        {
            var spending = _spendingFactory.Create(
                category.Id,
                DateTime.Now,
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            );

            _spendingRepository.GetAsync(spending.Id).Returns(spending);
            spendings.Add(spending);
        }

        var command = new DeleteSpendingsCommand(
            spendings.Select(s => s.Id).Union(new [] { Guid.NewGuid() }), 
            userId
        );
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(command, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<SpendingDoesNotExistException>();
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_ThrowsSpendingBelongsToAnotherUserException_WhenAtLeastOneSpendingBelongsToAnotherUser()
    {
        //Arrange
        var spendings = new List<Spending>();
        var userId = _fixture.Create<string>();
        
        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);

        for (int i = 0; i < 3; i++)
        {
            var spending = _spendingFactory.Create(
                category.Id,
                DateTime.Now,
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            );

            _spendingRepository.GetAsync(spending.Id).Returns(spending);
            spendings.Add(spending);
        }

        var command = new DeleteSpendingsCommand(spendings.Select(s => s.Id), userId);
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(command, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<SpendingBelongsToAnotherUserException>();
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_CallsRepository_ForEachIdToDelete()
    {
        //Arrange
        var spendings = new List<Spending>();
        var userId = _fixture.Create<string>();
        
        var category = _categoryFactory.Create(
            userId,
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);

        for (int i = 0; i < 3; i++)
        {
            var spending = _spendingFactory.Create(
                category.Id,
                DateTime.Now,
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            );

            _spendingRepository.GetAsync(spending.Id).Returns(spending);
            spendings.Add(spending);
        }

        var command = new DeleteSpendingsCommand(spendings.Select(s => s.Id), userId);
        var tokenSource = new CancellationTokenSource();

        //Act
        await _handler.Handle(command, tokenSource.Token);

        //Assert
        _unitOfWork.Received(1).BeginTransaction();

        foreach (var s in spendings)
            await _spendingRepository.Received(1).DeleteAsync(Arg.Is(s.Id));

        _unitOfWork.Received(1).Commit();
    }
}