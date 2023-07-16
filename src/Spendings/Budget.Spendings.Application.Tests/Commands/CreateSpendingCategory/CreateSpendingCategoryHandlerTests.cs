using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Domain.WriteModel.Entities;
using Budget.Spendings.Domain.WriteModel.Repositories;

using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Budget.Spendings.Application.Tests.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryHandlerTests
{
    private Fixture _fixture;

    private ISpendingCategoryRepository _repository;
    private IUnitOfWork _unitOfWork;
    private ILogger _logger;

    private CreateSpendingCategoryHandler _handler;

    public CreateSpendingCategoryHandlerTests()
    {
        _fixture = new Fixture();

        _repository = Substitute.For<ISpendingCategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.SpendingCategories.Returns(_repository);

        _logger = Substitute.For<ILogger>();       

        _handler = new CreateSpendingCategoryHandler(
            _unitOfWork,
            _logger
        );
    }

    [Fact]
    public void Handle_CreatesAndSaves_SpendingCategory()
    {
        //Arrange
        var command = _fixture.Build<CreateSpendingCategoryCommand>()
            .With(c => c.Amount, (new Random()).NextDouble() * 1000000)
            .Create();

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        //Act
        var task = _handler.Handle(command, token);
        task.Wait();
        var result = task.Result;

        //Assert
        result.Id.Should().NotBeEmpty();

        _unitOfWork.Received(1).BeginTransaction();
        _repository.Received(1).SaveAsync(
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
    public void Handle_RollbacksTransaction_WhenErrorOccur()
    {
        //Arrange
        var command = _fixture.Build<CreateSpendingCategoryCommand>()
            .With(c => c.Amount, (new Random()).NextDouble() * 1000000)
            .Create();

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