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
    public async Task Handle_GetsAndReturns_Spending()
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

        var request = new GetSpendingByIdCommand(spending.Id, userId);
        var tokenSource = new CancellationTokenSource();

        //Act
        var result = await _handler.Handle(request, tokenSource.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().Be(spending);
    }

    [Fact]
    public async Task Handle_ThrowsSpendingBelongsToAnotherUserException_WhenSpendingBelongsToAnotherUser()
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

        var request = new GetSpendingByIdCommand(spending.Id, userId);
        var tokenSource = new CancellationTokenSource();

        //Act
        var action = () => _handler.Handle(request, tokenSource.Token);

        //Assert
        await action.Should().ThrowAsync<SpendingBelongsToAnotherUserException>();
    }

}