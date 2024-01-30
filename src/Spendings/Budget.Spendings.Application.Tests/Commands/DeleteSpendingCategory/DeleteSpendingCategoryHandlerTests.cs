using Budget.Spendings.Application.Commands.DeleteSpendingCategory;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Domain.Repositories;

using AutoFixture;
using FluentAssertions;
using NSubstitute;

using Microsoft.Extensions.Logging;
using Budget.Spendings.Application.Exceptions;

namespace Budget.Spendings.Application.Tests.Commands.DeleteSpendingCategory;

public class DeleteSpendingCategoryHandlerTests
{
    private readonly Fixture _fixture;

    private readonly ISpendingCategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSpendingCategoryHandler> _logger;
    private readonly DeleteSpendingCategoryHandler _handler;
    private readonly SpendingCategoryFactory _factory;

    public DeleteSpendingCategoryHandlerTests()
    {
        _fixture = new Fixture();

        _repository = Substitute.For<ISpendingCategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.SpendingCategories.Returns(_repository);

        _logger = Substitute.For<ILogger<DeleteSpendingCategoryHandler>>();

        _handler = new DeleteSpendingCategoryHandler(_unitOfWork, _logger);

        _factory = new SpendingCategoryFactory();
    }

    [Fact]
    public async Task Handle_ThrowsArgumentNullException_WhenCommandIsNull()
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
    public async Task Handle_ThrowsCategoryDoesNotExistException_WhenCategoryIsNotFound()
    {
        //Arrange
        var command = new DeleteSpendingCategoryCommand(Guid.NewGuid(), _fixture.Create<string>());
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(command, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<CategoryDoesNotExistException>();
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_ThrowsCategoryBelongsToAnotherUserException_WhenCategoryIsNotFound()
    {
        //Arrange
        var category = _factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _repository.GetAsync(Arg.Is(category.Id)).Returns(category);

        var command = new DeleteSpendingCategoryCommand(category.Id, _fixture.Create<string>());
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(command, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<CategoryBelongsToAnotherUserException>();
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task Handle_DeletesCategory()
    {
        //Arrange
        var category = _factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _repository.GetAsync(Arg.Is(category.Id)).Returns(category);

        var command = new DeleteSpendingCategoryCommand(category.Id, category.UserId);
        var tokenSource = new CancellationTokenSource();

        //Act
        await _handler.Handle(command, tokenSource.Token);

        //Assert
        _unitOfWork.Received(1).BeginTransaction();
        await _repository.Received(1).DeleteAsync(Arg.Is(category));
        _unitOfWork.Received(1).Commit();
    }
}