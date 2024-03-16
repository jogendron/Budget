using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Repositories;

using AutoFixture;
using FluentAssertions;
using NSubstitute;

using Microsoft.Extensions.Logging;
using Budget.Spendings.Application.Queries.GetSpending;
using Budget.Spendings.Domain.Factories;

namespace Budget.Spendings.Application.Tests.Queries.GetSpending;

public class GetSpendingHandlerTests
{
    private readonly Fixture _fixture;
    private readonly SpendingCategoryFactory _categoryFactory;
    private readonly SpendingFactory _spendingFactory;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpendingCategoryRepository _categoryRepository;
    private readonly ISpendingRepository _spendingRepository;
    private readonly ILogger<GetSpendingHandler> _logger;
    private readonly GetSpendingHandler _handler;

    public GetSpendingHandlerTests()
    {
        _fixture = new Fixture();
        _categoryFactory = new SpendingCategoryFactory();
        _spendingFactory = new SpendingFactory();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _categoryRepository = Substitute.For<ISpendingCategoryRepository>();
        _spendingRepository = Substitute.For<ISpendingRepository>();

        _logger = Substitute.For<ILogger<GetSpendingHandler>>();

        _unitOfWork.SpendingCategories.Returns(_categoryRepository);
        _unitOfWork.Spendings.Returns(_spendingRepository);

        _handler = new GetSpendingHandler(_unitOfWork, _logger);
    }
    
    [Fact]
    public async Task GetBySpendingId_GetsAndReturns_Spending()
    {
        //Arrange
        var userId = _fixture.Create<string>();

        var category = _categoryFactory.Create(
            userId,
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            _fixture.Create<double>(),
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

        var request = new GetSpendingByIdCommand(userId, spending.Id);
        var tokenSource = new CancellationTokenSource();

        //Act
        var result = await _handler.Handle(request, tokenSource.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().Be(spending);
    }

    [Fact]
    public async Task GetBySpendingId_ThrowsSpendingBelongsToAnotherUserException_WhenSpendingBelongsToAnotherUser()
    {
        //Arrange
        var userId = _fixture.Create<string>();

        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            _fixture.Create<double>(),
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

        var request = new GetSpendingByIdCommand(userId, spending.Id);
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = () => _handler.Handle(request, tokenSource.Token);

        //Assert
        await action.Should().ThrowAsync<SpendingBelongsToAnotherUserException>();
    }

    [Fact]
    public async Task GetByCategoryId_GetsAndReturns_AllSpendingsFromCategory()
    {
        //Arrange
        var userId = _fixture.Create<string>();

        var category = _categoryFactory.Create(
            userId,
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var spending1 = _spendingFactory.Create(
            category.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var spending2 = _spendingFactory.Create(
            category.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);
        _spendingRepository.GetAsync(
            Arg.Is(category.Id),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>()
        ).Returns(new [] { spending1, spending2 });

        var command = new GetSpendingsByCategoryCommand(userId, category.Id, null, null);
        var tokenSource = new CancellationTokenSource();

        //Act
        var result = await _handler.Handle(command, tokenSource.Token);

        //Assert
        result.Should().NotBeNullOrEmpty();
        result.Count().Should().Be(2);
        result.Should().Contain(spending1);
        result.Should().Contain(spending2);
    }

    [Fact]
    public async Task GetByCategoryId_ThrowsCategoryBelongsToAnotherUser_WhenCategoryBelongsToAnotherUser()
    {
        //Arrange
        var userId = _fixture.Create<string>();

        var category = _categoryFactory.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Frequency>(),
            _fixture.Create<double>(),
            _fixture.Create<string>()
        );

        var spending1 = _spendingFactory.Create(
            category.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        var spending2 = _spendingFactory.Create(
            category.Id,
            DateTime.Now,
            new Random().NextDouble() * 10000,
            _fixture.Create<string>()
        );

        _categoryRepository.GetAsync(Arg.Is(category.Id)).Returns(category);
        _spendingRepository.GetAsync(
            Arg.Is(category.Id),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>()
        ).Returns(new [] { spending1, spending2 });

        var command = new GetSpendingsByCategoryCommand(userId, category.Id, null, null);
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = () => _handler.Handle(command, tokenSource.Token);

        //Assert
        await action.Should().ThrowAsync<CategoryBelongsToAnotherUserException>();
    }

    [Fact]
    public async Task HandleGetByUser_CallsRepository()
    {
        //Arrange
        var spendings = new [] {
            _spendingFactory.Create(
                _fixture.Create<Guid>(),
                DateTime.Now,
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            ),
            _spendingFactory.Create(
                _fixture.Create<Guid>(),
                DateTime.Now,
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            )
        };

        var command = new GetSpendingsByUserCommand(
            _fixture.Create<string>(),
            DateTime.MinValue,
            DateTime.MaxValue
        );

        _spendingRepository.GetAsync(
            Arg.Is(command.UserId),
            Arg.Is(command.BeginDate),
            Arg.Is(command.EndDate)
        ).Returns(spendings);

        var tokenSource = new CancellationTokenSource();

        //Act
        var result = await _handler.Handle(command, tokenSource.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(spendings);
    }
}