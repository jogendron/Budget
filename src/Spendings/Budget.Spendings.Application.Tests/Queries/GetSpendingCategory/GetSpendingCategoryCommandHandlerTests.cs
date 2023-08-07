using Budget.Spendings.Application.Queries.GetSpendingCategory;
using Budget.Spendings.Domain.ReadModel.Entities;
using Budget.Spendings.Domain.ReadModel.Repositories;

using AutoFixture;
using FluentAssertions;
using NSubstitute;

namespace Budget.Spendings.Application.Tests.Queries.GetSpendingCategory;

public class GetSpendingCategoryCommandHandlerTests
{
    private readonly Fixture _fixture;
    private ISpendingCategoryRepository _repository;
    private GetSpendingCategoryCommandHandler _handler;

    public GetSpendingCategoryCommandHandlerTests()
    {
        _fixture = new Fixture();
        _repository = Substitute.For<ISpendingCategoryRepository>();
        _handler = new GetSpendingCategoryCommandHandler(_repository);
    }

    private SpendingCategory CreateSpendingCategory()
    {
        return new SpendingCategory(
            Guid.NewGuid(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );
    }

    [Fact]
    public async Task Handle_GetById_ReturnsCategoryFromRepository()
    {
        //Arrange
        var command = _fixture.Create<GetSpendingCategoryByIdCommand>();
        var expectedCategory = CreateSpendingCategory();

        _repository.Get(Arg.Is(command.Id)).Returns(expectedCategory);

        var tokenSource = new CancellationTokenSource();

        //Act
        var category = await _handler.Handle(command, tokenSource.Token);

        //Assert
        category.Should().NotBeNull();
        category.Should().BeEquivalentTo(expectedCategory);
    }

    [Fact]
    public async Task Handle_GetByUserAndName_ReturnsCategoryFromRepository()
    {
        //Arrange
        var command = _fixture.Create<GetSpendingCategoryByUserAndNameCommand>();
        var expectedCategory = CreateSpendingCategory();

        _repository.Get(
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

        _repository.Get(Arg.Is(command.UserId)).Returns(expectedCategories);

        var tokenSource = new CancellationTokenSource();

        //Act
        var categories = await _handler.Handle(command, tokenSource.Token);

        //Assert
        categories.Should().NotBeNull();
        categories.Should().BeEquivalentTo(expectedCategories);
    }
}