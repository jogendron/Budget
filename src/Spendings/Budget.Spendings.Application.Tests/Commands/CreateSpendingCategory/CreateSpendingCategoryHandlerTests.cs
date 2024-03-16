using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Domain.Repositories;

using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

using Budget.Spendings.Application.Exceptions;

namespace Budget.Spendings.Application.Tests.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryHandlerTests
{
    private Fixture _fixture;

    private ISpendingCategoryRepository _repository;
    private IUnitOfWork _unitOfWork;
    private ILogger<CreateSpendingCategoryHandler> _logger;

    private CreateSpendingCategoryHandler _handler;

    public CreateSpendingCategoryHandlerTests()
    {
        _fixture = new Fixture();

        _repository = Substitute.For<ISpendingCategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.SpendingCategories.Returns(_repository);

        _logger = Substitute.For<ILogger<CreateSpendingCategoryHandler>>();       

        _handler = new CreateSpendingCategoryHandler(
            _unitOfWork,
            _logger
        );
    }

    private CreateSpendingCategoryCommand CreateCommand()
    {
        return new CreateSpendingCategoryCommand(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );
    }

    [Fact]
    public async Task Handle_CreatesAndSaves_SpendingCategory()
    {
        //Arrange
        var command = CreateCommand();

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        //Act
        var result = await _handler.Handle(command, token);

        //Assert
        result.Id.Should().NotBeEmpty();

        _unitOfWork.Received(1).BeginTransaction();
        await _repository.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.UserId == command.UserId
                && s.Name == command.Name
                && s.Frequency == command.Frequency
                && s.Amount == command.Amount
                && s.Description == command.Description
            )
        );
        _unitOfWork.Received(1).Commit();
    }

    [Fact]
    public async Task Handle_ThrowsSpendingCategoryAlreadyExistsException_WhenCategoryExists()
    {
        //Arrange
        var factory = new SpendingCategoryFactory();
        var existingCategory = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            (new Random()).NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        var command = CreateCommand();

        _repository.GetAsync(
            Arg.Is(command.UserId), 
            Arg.Is(command.Name)
        ).Returns(existingCategory);

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        //Act
        var action = (() => _handler.Handle(command, token));

        //Assert
        await action.Should().ThrowAsync<SpendingCategoryAlreadyExistsException>();
    }

    [Fact]
    public void Handle_RollbacksTransaction_WhenErrorOccur()
    {
        //Arrange
        var command = CreateCommand();

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        _repository.When(
            s => s.SaveAsync(Arg.Any<SpendingCategory>())
        ).Do(args => throw new ArgumentException());

        //Act
        var action = (() => _handler.Handle(command, token));

        //Assert
        action.Should().ThrowAsync<ArgumentException>();

        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Rollback();
    }
    
}