using Budget.Spendings.Application.Commands.UpdateSpendingCategory;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Repositories;

using AutoFixture;
using FluentAssertions;
using NSubstitute;

using Microsoft.Extensions.Logging;
using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Domain.Events;

namespace Budget.Spendings.Application.Tests.Commands.UpdateSpendingCategory;

public class UpdateSpendingCategoryHandlerTests
{
    private Fixture _fixture;

    private IUnitOfWork _unitOfWork;
    private ILogger<UpdateSpendingCategoryHandler> _logger;
    private UpdateSpendingCategoryHandler _handler;

    public UpdateSpendingCategoryHandlerTests()
    {
        _fixture = new Fixture();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<UpdateSpendingCategoryHandler>>();

        _handler = new UpdateSpendingCategoryHandler(_unitOfWork, _logger);
    }

    [Fact]
    public async Task Handle_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = async () => await _handler.Handle(null!, tokenSource.Token);

        //Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_ThrowCategoryDoesNotExistException_WhenCategoryDoesNotExist()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var request = new UpdateSpendingCategoryCommand(
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            null,
            (new Random()).NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        //Act
        var action = async () => await _handler.Handle(request, tokenSource.Token);

        //Assert
        await action.Should().ThrowAsync<CategoryDoesNotExistException>();
    }

    [Fact]
    public async Task Handle_ThrowsCategoryBelongsToAnotherUserException_WhenCategoryBelongsToAnotherUser()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId + "1",
            category.Id,
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            null,
            (new Random()).NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        var action = async () => await _handler.Handle(request, tokenSource.Token);

        //Assert
        await action.Should().ThrowAsync<CategoryBelongsToAnotherUserException>();
    }

    [Fact]
    public async Task Handle_ThrowsSpendingCategoryAlreadyExistsException_WhenCategoryIsRenamedToAnExistingCategory()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var category1 = factory.Create(
            category.UserId,
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            category1.Name,
            _fixture.Create<Domain.Entities.Frequency>(),
            null,
            (new Random()).NextDouble() * 1000000,
            _fixture.Create<string>()
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(category1.UserId),
            Arg.Is(category1.Name)
        ).Returns(category1);

        //Act
        var action = async () => await _handler.Handle(request, tokenSource.Token);

        //Assert
        await action.Should().ThrowAsync<SpendingCategoryAlreadyExistsException>(); 
    }

    [Fact]
    public async Task Handle_SendsRequestNameToUpdate_WhenRequestHasName()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            _fixture.Create<string>(),
            null,
            null,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryUpdated)
                        && ((SpendingCategoryUpdated)c).Name == request.Name
                )
            )
        );
    }

    [Fact]
    public async Task Handle_SendsCategoryNameToUpdate_WhenRequestHasNoName()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            null,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryUpdated)
                        && ((SpendingCategoryUpdated)c).Name == category.Name
                )
            )
        );
    }

    [Fact]
    public async Task Handle_SendsRequestFrequencyToUpdate_WhenRequestHasFrequency()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            Domain.Entities.Frequency.Daily,
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            Domain.Entities.Frequency.Monthly,
            null,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryUpdated)
                        && ((SpendingCategoryUpdated)c).Frequency == Domain.Events.Frequency.Monthly
                )
            )
        );
    }

    [Fact]
    public async Task Handle_SendsCategoryFrequencyToUpdate_WhenRequestHasNoFrequency()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            Domain.Entities.Frequency.Daily,
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            null,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryUpdated)
                        && ((SpendingCategoryUpdated)c).Frequency == Domain.Events.Frequency.Daily
                )
            )
        );
    }

    [Fact]
    public async Task Handle_SendsRequestAmountToUpdate_WhenRequestHasAmount()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            null,
            (new Random()).NextDouble() * 1000000,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryUpdated)
                        && ((SpendingCategoryUpdated)c).Amount == request.Amount
                )
            )
        );
    }

    [Fact]
    public async Task Handle_SendsCategoryAmountToUpdate_WhenRequestHasNoAmount()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            null,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryUpdated)
                        && ((SpendingCategoryUpdated)c).Amount == category.Amount
                )
            )
        );
    }

    [Fact]
    public async Task Handle_SendsRequestDescriptionToUpdate_WhenRequestHasDescription()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            null,
            null,
            _fixture.Create<string>()
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryUpdated)
                        && ((SpendingCategoryUpdated)c).Description == request.Description
                )
            )
        );
    }

    [Fact]
    public async Task Handle_SendsCategoryDescriptionToUpdate_WhenRequestHasNoDescription()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            null,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryUpdated)
                        && ((SpendingCategoryUpdated)c).Description == category.Description
                )
            )
        );
    }

    [Fact]
    public async Task Handle_OpensPeriod_WhenRequestHasOpenedPeriodAndCategoryHasClosedPeriod()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );
        category.ClosePeriod();

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            true,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryPeriodOpened)
                )
                && s.Period.EndDate == null
            )
        );
    }

    [Fact]
    public async Task Handle_ClosesPeriod_WhenRequestHasClosedPeriodAndCategoryHasOpenedPeriod()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            false,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        await _unitOfWork.SpendingCategories.Received(1).SaveAsync(
            Arg.Is<SpendingCategory>(
                s => s.Changes.Any(
                    c => c.GetType() == typeof(SpendingCategoryPeriodClosed)
                )
                && s.Period.EndDate != null
            )
        );
    }

    [Fact]
    public async Task Handle_CommitsTransaction_WhenNoProblemOccurs()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            false,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        //Act
        await _handler.Handle(request, tokenSource.Token);

        //Assert
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(1).Commit();
    }

    [Fact]
    public async Task Handle_RollbacksTransaction_WhenExceptionOccurs()
    {
        //Arrange
        var tokenSource = new CancellationTokenSource();

        var factory = new SpendingCategoryFactory();
        var category = factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Domain.Entities.Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var request = new UpdateSpendingCategoryCommand(
            category.UserId,
            category.Id,
            null,
            null,
            false,
            null,
            null
        );

        _unitOfWork.SpendingCategories.GetAsync(
            Arg.Is(request.SpendingCategoryId)
        ).Returns(category);

        _unitOfWork.When(
            u => u.BeginTransaction()
        ).Do(c => throw new ArgumentException());

        //Act
        var action = async () => await _handler.Handle(request, tokenSource.Token);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
        _unitOfWork.Received(1).BeginTransaction();
        _unitOfWork.Received(0).Commit();
        _unitOfWork.Received(1).Rollback();
    }
}