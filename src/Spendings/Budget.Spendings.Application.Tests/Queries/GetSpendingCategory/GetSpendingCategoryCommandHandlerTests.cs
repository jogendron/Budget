using Budget.Spendings.Application.Queries.GetSpendingCategory;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Repositories;

using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Budget.Spendings.Application.Tests.Queries.GetSpendingCategory;

public class GetSpendingCategoryCommandHandlerTests
{
    private readonly Fixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpendingCategoryRepository _repository;
    private readonly ILogger<GetSpendingCategoryHandler> _logger;
    private readonly GetSpendingCategoryHandler _handler;
    private readonly SpendingCategoryFactory _factory;

    public GetSpendingCategoryCommandHandlerTests()
    {
        _fixture = new Fixture();
        _repository = Substitute.For<ISpendingCategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.SpendingCategories.Returns(_repository);
        _logger = Substitute.For<ILogger<GetSpendingCategoryHandler>>();
        _handler = new GetSpendingCategoryHandler(_unitOfWork, _logger);

        _factory = new SpendingCategoryFactory();
    }

    private SpendingCategory CreateSpendingCategory()
    {
        return _factory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );
    }

    [Fact]
    public async Task Handle_GetById_ReturnsCategoryFromRepository()
    {
        //Arrange
        var expectedCategory = CreateSpendingCategory();
        var command = new GetSpendingCategoryByIdCommand(
            expectedCategory.Id,
            expectedCategory.UserId
        );

        _repository.GetAsync(Arg.Is(command.Id)).Returns(expectedCategory);

        var tokenSource = new CancellationTokenSource();

        //Act
        var category = await _handler.Handle(command, tokenSource.Token);

        //Assert
        category.Should().NotBeNull();
        category.Should().BeEquivalentTo(expectedCategory);
    }

    [Fact]
    public async Task Handle_GetById_ThrowsCategoryBelongsToAnotherUserException_WhenCategoryBelongsToAnotherUser()
    {
        //Arrange
        var command = _fixture.Create<GetSpendingCategoryByIdCommand>();
        var expectedCategory = CreateSpendingCategory();

        _repository.GetAsync(Arg.Is(command.Id)).Returns(expectedCategory);

        var tokenSource = new CancellationTokenSource();

        //Act
        var action = (() => _handler.Handle(command, tokenSource.Token));

        //Assert
        await action.Should().ThrowAsync<CategoryBelongsToAnotherUserException>();
    }

    [Fact]
    public async Task Handle_GetByUserAndName_ReturnsCategoryFromRepository()
    {
        //Arrange
        var command = _fixture.Create<GetSpendingCategoryByUserAndNameCommand>();
        var expectedCategory = CreateSpendingCategory();

        _repository.GetAsync(
            Arg.Is(command.UserId), 
            Arg.Is(command.Name)
        ).Returns(expectedCategory);

        var tokenSource = new CancellationTokenSource();

        //Act
        var category = await _handler.Handle(command, tokenSource.Token);

        //Assert
        category.Should().NotBeNull();
        category.Should().BeEquivalentTo(expectedCategory);
    }

    [Fact]
    public async Task Handle_GetByUser_ReturnsCategoriesFromRepository()
    {
        //Arrange
        var command = _fixture.Create<GetSpendingCategoriesByUserCommand>();

        var expectedCategories = new List<SpendingCategory>()
        {
            CreateSpendingCategory(),
            CreateSpendingCategory(),
            CreateSpendingCategory()
        };

        _repository.GetAsync(Arg.Is(command.UserId)).Returns(expectedCategories);

        var tokenSource = new CancellationTokenSource();

        //Act
        var categories = await _handler.Handle(command, tokenSource.Token);

        //Assert
        categories.Should().NotBeNull();
        categories.Should().BeEquivalentTo(expectedCategories);
    }
}