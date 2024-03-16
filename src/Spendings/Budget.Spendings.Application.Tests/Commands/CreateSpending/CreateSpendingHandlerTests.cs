using Budget.Spendings.Application.Commands.CreateSpending;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Domain.Repositories;

using Microsoft.Extensions.Logging;

using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Entities;

namespace Budget.Spendings.Application.Tests.Commands.CreateSpending;

public class CreateSpendingHandlerTests
{
    private readonly Fixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpendingRepository _spendingRepository;
    private readonly ISpendingCategoryRepository _categoryRepository;
    private readonly ILogger<CreateSpendingHandler> _logger;
    private readonly CreateSpendingHandler _handler;

    public CreateSpendingHandlerTests()
    {
        _fixture = new Fixture();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _spendingRepository = Substitute.For<ISpendingRepository>();
        _categoryRepository = Substitute.For<ISpendingCategoryRepository>();

        _unitOfWork.Spendings.Returns(_spendingRepository);
        _unitOfWork.SpendingCategories.Returns(_categoryRepository);

        _logger = Substitute.For<ILogger<CreateSpendingHandler>>();

        _handler = new CreateSpendingHandler(
            _unitOfWork, 
            _logger
        );
    }

    [Fact]
    public async Task Handle_ThrowsCategoryDoesNotExistException_WhenCategoryDoesNotExist()
    {
        //Arrange
        var request = new CreateSpendingCommand(
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<DateTime>(),
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(request, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<CategoryDoesNotExistException>();
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_ThrowsCategoryBelongsToAnotherUserException_WhenCategoryBelongsToAnotherUser()
    {
        //Arrange
        var request = new CreateSpendingCommand(
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<DateTime>(),
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var categoryFactory = new SpendingCategoryFactory();
        var category = categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            Domain.Entities.Frequency.Monthly,
            new Random().NextDouble() * 1000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(
            Arg.Is(request.CategoryId)
        ).Returns(category);

        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(request, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<CategoryBelongsToAnotherUserException>();
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_CreateAndSaves_Spending()
    {
        //Arrange
        var userId = _fixture.Create<string>();

        var categoryFactory = new SpendingCategoryFactory();

        var category = categoryFactory.Create(
            userId,
            _fixture.Create<string>(),
            Frequency.Monthly,
            new Random().NextDouble() * 1000,
            _fixture.Create<string>()
        );

        var request = new CreateSpendingCommand(
            userId,
            category.Id,
            _fixture.Create<DateTime>(),
            new Random().NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(
            Arg.Is(request.CategoryId)
        ).Returns(category);

        var tokenSource = new CancellationTokenSource();

        //Act
        var response = await _handler.Handle(request, tokenSource.Token);

        //Assert
        _unitOfWork.Received(1).BeginTransaction();
        
        await _spendingRepository.Received(1).SaveAsync(
            Arg.Is<Spending>(
                s => s.CategoryId == request.CategoryId
                && s.Date == request.Date
                && s.Amount == request.Amount
                && s.Description == request.Description
            )
        );

        response.Id.Should().NotBeEmpty();

        _unitOfWork.Received(1).Commit();
    }
}