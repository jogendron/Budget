using Budget.Spendings.Api.Controllers;
using Budget.Spendings.Api.Models;
using Budget.Spendings.Api.Services;
using Budget.Spendings.Application.Commands.CreateSpending;
using Budget.Spendings.Application.Commands.UpdateSpending;
using Budget.Spendings.Application.Queries.GetSpending;

using MediatR;
using Microsoft.Extensions.Logging;

using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Budget.Spendings.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Application.Commands.DeleteSpending;

namespace Budget.Spendings.Api.Tests.Controllers;

public class SpendingsControllerTests
{
    private readonly Fixture _fixture;
    private readonly SpendingCategoryFactory _categoryFactory;
    private readonly SpendingFactory _spendingFactory;
    private readonly IMediator _mediator;
    private readonly IUserInspector _userInspector;
    private readonly ILogger<SpendingsController> _logger;
    private readonly SpendingsController _controller;

    public SpendingsControllerTests()
    {
        _fixture = new Fixture();
        _categoryFactory = new SpendingCategoryFactory();
        _spendingFactory = new SpendingFactory();

        _mediator = Substitute.For<IMediator>();
        _userInspector = Substitute.For<IUserInspector>();
        _logger = Substitute.For<ILogger<SpendingsController>>();

        _controller = new SpendingsController(_mediator, _userInspector, _logger);
    }

    [Fact]
    public async Task CreateSpending_Returns_CreatedAtRouteResult()
    {
        //Arrange
        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        var expectedResponse = new CreateSpendingResponse(Guid.NewGuid());
        _mediator.Send(Arg.Any<CreateSpendingCommand>()).Returns(expectedResponse);

        var spending = _fixture.Create<NewSpending>();

        //Act
        var response = await _controller.CreateSpending(spending);

        //Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<CreatedAtRouteResult>();

        var createResponse = response as CreatedAtRouteResult;
        createResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateSpending_ReturnsBadRequest_WhenCategoryDoesNotExist()
    {
        //Arrange
        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(
            m => m.Send(Arg.Any<CreateSpendingCommand>())
        ).Do(c => throw new CategoryDoesNotExistException());

        var spending = _fixture.Create<NewSpending>();

        //Act
        var result = await _controller.CreateSpending(spending);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task CreateSpending_ReturnsBadRequest_WhenCategoryBelongsToAnotherUser()
    {
        //Arrange
        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(
            m => m.Send(Arg.Any<CreateSpendingCommand>())
        ).Do(c => throw new CategoryBelongsToAnotherUserException());

        var spending = _fixture.Create<NewSpending>();

        //Act
        var result = await _controller.CreateSpending(spending);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task CreateSpending_ReturnsInternalError_WhenUnexpectedErrorOccurs()
    {
        //Arrange
        _mediator.When(
            m => m.Send(Arg.Any<CreateSpendingCommand>())
        ).Do(c => throw new DivideByZeroException());

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        var spending = _fixture.Create<NewSpending>();

        //Act
        var result = await _controller.CreateSpending(spending);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<StatusCodeResult>();
        
        var statusResult = result as StatusCodeResult;
        if (statusResult != null)
        {
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }

    [Fact]
    public async Task GetSpendingFromId_ReturnsOkObjectResult()
    {
        //Arrange
        var spending = _spendingFactory.Create(
            Guid.NewGuid(),
            _fixture.Create<DateTime>(),
            new Random().NextDouble() * 1000,
            _fixture.Create<string>()
        );

        var userId = _fixture.Create<string>();
        _userInspector.GetAuthenticatedUser().Returns(userId);

        _mediator.Send(
            Arg.Is<GetSpendingByIdCommand>(c => 
                c.Id == spending.Id
                && c.UserId == userId
            )
        ).Returns(spending);

        //Act
        var result = await _controller.GetSpending(spending.Id);

        //Assert
        result.Should().NotBeNull();

        result.Should().BeOfType<OkObjectResult>();

        var objectResult = result as OkObjectResult;
        if (objectResult != null)
        {
            objectResult.Value.Should().NotBeNull();
            var spendingResult = objectResult.Value as Models.Spending;

            if (spendingResult != null)
            {
                spendingResult.Id.Should().Be(spending.Id);
                spendingResult.Date.Should().Be(spending.Date);
                spendingResult.Amount.Should().Be(spending.Amount);
                spendingResult.Description.Should().Be(spending.Description);
            }
        }
    }

    [Fact]
    public async Task GetSpendingById_ReturnsNotFound_WhenSpendingIsNotFound()
    {
        //Arrange
        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        //Act
        var result = await _controller.GetSpending(Guid.NewGuid());

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSpendingById_ReturnsNotFound_WhenSpendingBelongsToAnotherUser()
    {
        //Arrange
        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(
            m => m.Send(Arg.Any<GetSpendingByIdCommand>())
        ).Do(c => throw new SpendingBelongsToAnotherUserException());

        //Act
        var result = await _controller.GetSpending(Guid.NewGuid());

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSpendingById_ReturnsInternalError_WhenUnexpectedExceptionOccurs()
    {
        //Arrange
        _mediator.When(
            m => m.Send(Arg.Any<GetSpendingByIdCommand>())
        ).Do(c => throw new DivideByZeroException());

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        //Act
        var result = await _controller.GetSpending(Guid.NewGuid());

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<StatusCodeResult>();
        var statusResult = result as StatusCodeResult;

        if (statusResult != null)
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetSpendingsByCategory_ReturnsOkObjectResult()
    {
        //Arrange
        var categoryId = Guid.NewGuid();
        var beginDate = DateTime.MinValue;;
        var endDate = DateTime.MaxValue;

        var userId = _fixture.Create<string>();
        _userInspector.GetAuthenticatedUser().Returns(userId);

        var spendings = new [] {
            _spendingFactory.Create(
                categoryId,
                _fixture.Create<DateTime>(),
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            ),
            _spendingFactory.Create(
                categoryId,
                _fixture.Create<DateTime>(),
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            )
        };

        _mediator.Send(
            Arg.Is<GetSpendingsByCategoryCommand>(
                c => c.CategoryId == categoryId
                && c.UserId == userId
                && c.BeginDate == beginDate
                && c.EndDate == endDate
            )
        ).Returns(spendings);

        //Act
        var result = await _controller.GetSpendings(categoryId, beginDate, endDate);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;

        if (okResult != null)
        {
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeAssignableTo<IEnumerable<Models.Spending>>();

            var value = okResult.Value as IEnumerable<Models.Spending>;
            if (value != null)
            {
                value.Count().Should().Be(2);

                value.First().Id.Should().NotBeEmpty();
                value.First().CategoryId.Should().Be(categoryId);
                value.First().Date.Should().Be(spendings.First().Date);
                value.First().Amount.Should().Be(spendings.First().Amount);
                value.First().Description.Should().Be(spendings.First().Description);

                value.Take(1).First().Id.Should().NotBeEmpty();
                value.Take(1).First().CategoryId.Should().Be(categoryId);
                value.Take(1).First().Date.Should().Be(spendings.Take(1).First().Date);
                value.Take(1).First().Amount.Should().Be(spendings.Take(1).First().Amount);
                value.Take(1).First().Description.Should().Be(spendings.Take(1).First().Description);
            }
        }
    }

    [Fact]
    public async Task GetSpendingsByCategory_ReturnsNotFound_WhenNoCategoryIsFound()
    {
        //Arrange
        var categoryId = Guid.NewGuid();

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        //Act
        var result = await _controller.GetSpendings(categoryId, null, null);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSpendingsByUser_ReturnsOkObjectResult()
    {
        //Arrange
        var beginDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

        var userId = _fixture.Create<string>();
        _userInspector.GetAuthenticatedUser().Returns(userId);

        var spendings = new [] {
            _spendingFactory.Create(
                Guid.NewGuid(),
                _fixture.Create<DateTime>(),
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            ),
            _spendingFactory.Create(
                Guid.NewGuid(),
                _fixture.Create<DateTime>(),
                new Random().NextDouble() * 10000,
                _fixture.Create<string>()
            )
        };

        _mediator.Send(
            Arg.Is<GetSpendingsByUserCommand>(c =>
                c.UserId == userId
                && c.BeginDate == beginDate
                && c.EndDate == endDate
            )
        ).Returns(spendings);

        //Act
        var result = await _controller.GetSpendings(null, beginDate, endDate);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;

        if (okResult != null)
        {
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeAssignableTo<IEnumerable<Spending>>();

            var value = okResult.Value as IEnumerable<Spending>;
            if (value != null)
            {
                value.Count().Should().Be(2);

                value.First().Id.Should().NotBeEmpty();
                value.First().Date.Should().Be(spendings.First().Date);
                value.First().Amount.Should().Be(spendings.First().Amount);
                value.First().Description.Should().Be(spendings.First().Description);

                value.Take(1).First().Id.Should().NotBeEmpty();
                value.Take(1).First().Date.Should().Be(spendings.Take(1).First().Date);
                value.Take(1).First().Amount.Should().Be(spendings.Take(1).First().Amount);
                value.Take(1).First().Description.Should().Be(spendings.Take(1).First().Description);
            }
        }
    }

    [Fact]
    public async Task GetSpendingsByUser_ReturnsNotFound_WhenNoSpendingIsFound()
    {
        //Arrange
        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());
        var beginDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

        //Act
        var result = await _controller.GetSpendings(null, beginDate, endDate);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSpendingsByUser_ReturnsNotFound_WhenAskedForAnotherUserSpendings()
    {
        //Arrange
        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());
        var beginDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

        _mediator.When(
            m => m.Send(Arg.Any<GetSpendingsByUserCommand>())
        ).Do(
            c => throw new CategoryBelongsToAnotherUserException()
        );

        //Act
        var result = await _controller.GetSpendings(null, beginDate, endDate);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSpendingsByUser_ReturnsInternalError_WhenUnexpectedExceptionOccurs()
    {
        //Arrange
        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());
        var beginDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

        _mediator.When(
            m => m.Send(Arg.Any<GetSpendingsByUserCommand>())
        ).Do(
            c => throw new DivideByZeroException()
        );

        //Act
        var result = await _controller.GetSpendings(null, beginDate, endDate);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<StatusCodeResult>();

        var statusResult = result as StatusCodeResult;
        if (statusResult != null)
        {
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }

    [Fact]
    public async Task UpdateSpending_SendsCommandToMediator_AndReturnsOk()
    {
        //Arrange
        var spending = _fixture.Create<SpendingUpdate>();
        var userId = _fixture.Create<string>();
        _userInspector.GetAuthenticatedUser().Returns(userId);

        //Act
        var result = await _controller.UpdateSpending(spending);

        //Assert
        await _mediator.Received(1).Send(
            Arg.Is<UpdateSpendingCommand>(c =>
                c.SpendingId == spending.Id
                && c.UserId == userId
                && c.Date == spending.Date
                && c.Amount == spending.Amount
                && c.Description == spending.Description
            )
        );
        
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task UpdateSpending_ReturnsBadRequest_WhenArgumentNullExceptionOccurs()
    {
        //Arrange
        var spending = _fixture.Create<SpendingUpdate>();

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(m => 
            m.Send(Arg.Any<UpdateSpendingCommand>())
        ).Do(c => throw new ArgumentNullException());

        //Act
        var result = await _controller.UpdateSpending(spending);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task UpdateSpending_ReturnsNotFound_WhenSpendingBelongsToAnotherUserExceptionOccurs()
    {
        //Arrange
        var spending = _fixture.Create<SpendingUpdate>();

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(m => 
            m.Send(Arg.Any<UpdateSpendingCommand>())
        ).Do(c => throw new SpendingBelongsToAnotherUserException());

        //Act
        var result = await _controller.UpdateSpending(spending);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateSpending_ReturnsBadRequest_WhenCategoryDoesNotExistExceptionOccurs()
    {
        //Arrange
        var spending = _fixture.Create<SpendingUpdate>();

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(m => 
            m.Send(Arg.Any<UpdateSpendingCommand>())
        ).Do(c => throw new CategoryDoesNotExistException());

        //Act
        var result = await _controller.UpdateSpending(spending);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task UpdateSpending_ReturnsBadRequest_WhenCategoryBelongsToAnotherUserExceptionOccurs()
    {
        //Arrange
        var spending = _fixture.Create<SpendingUpdate>();

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(m => 
            m.Send(Arg.Any<UpdateSpendingCommand>())
        ).Do(c => throw new CategoryBelongsToAnotherUserException());

        //Act
        var result = await _controller.UpdateSpending(spending);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task UpdateSpending_ReturnsBadRequest_WhenSpendingDoesNotExistExceptionOccurs()
    {
        //Arrange
        var spending = _fixture.Create<SpendingUpdate>();

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(m => 
            m.Send(Arg.Any<UpdateSpendingCommand>())
        ).Do(c => throw new SpendingDoesNotExistException());

        //Act
        var result = await _controller.UpdateSpending(spending);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateSpending_ReturnsInternalError_WhenUnexpectedExceptionOccurs()
    {
        //Arrange
        var spending = _fixture.Create<SpendingUpdate>();

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _mediator.When(m => 
            m.Send(Arg.Any<UpdateSpendingCommand>())
        ).Do(c => throw new DivideByZeroException());

        //Act
        var result = await _controller.UpdateSpending(spending);

        //Assert
        result.Should().BeOfType<StatusCodeResult>();

        var statusResult = result as StatusCodeResult;
        if (statusResult != null)
        {
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }

    [Fact]
    public async Task DeleteSpending_ReturnsBadRequest_WhenArgumentNullExceptionOccurs()
    {
        //Arrange
        var id = Guid.NewGuid();
        var userId = _fixture.Create<string>();

        _userInspector.GetAuthenticatedUser().Returns(userId);

        _mediator.When(m => 
            m.Send(
                Arg.Is<DeleteSpendingCommand>(c => 
                    c.Id == id
                    && c.UserId == userId
                )
            )
        ).Do(c => throw new ArgumentNullException());

        //Act
        var result = await _controller.DeleteSpending(id);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestResult>();

    }

    [Fact]
    public async Task DeleteSpending_ReturnsNotFound_WhenSpendingDoesNotExist()
    {
        //Arrange
        var id = Guid.NewGuid();
        var userId = _fixture.Create<string>();

        _userInspector.GetAuthenticatedUser().Returns(userId);

        _mediator.When(m => 
            m.Send(
                Arg.Is<DeleteSpendingCommand>(c => 
                    c.Id == id
                    && c.UserId == userId
                )
            )
        ).Do(c => throw new SpendingDoesNotExistException());

        //Act
        var result = await _controller.DeleteSpending(id);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();

    }

    [Fact]
    public async Task DeleteSpending_ReturnsNotFound_WhenSpendingBelongsToAnotherUser()
    {
        //Arrange
        var id = Guid.NewGuid();
        var userId = _fixture.Create<string>();

        _userInspector.GetAuthenticatedUser().Returns(userId);

        _mediator.When(m => 
            m.Send(
                Arg.Is<DeleteSpendingCommand>(c => 
                    c.Id == id
                    && c.UserId == userId
                )
            )
        ).Do(c => throw new SpendingBelongsToAnotherUserException());

        //Act
        var result = await _controller.DeleteSpending(id);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteSpending_ReturnsInternalError_WhenUnexpectedExceptionOccurs()
    {
        //Arrange
        var id = Guid.NewGuid();
        var userId = _fixture.Create<string>();

        _userInspector.GetAuthenticatedUser().Returns(userId);

        _mediator.When(m => 
            m.Send(
                Arg.Is<DeleteSpendingCommand>(c => 
                    c.Id == id
                    && c.UserId == userId
                )
            )
        ).Do(c => throw new DivideByZeroException());

        //Act
        var result = await _controller.DeleteSpending(id);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<StatusCodeResult>();

        var statusResult = result as StatusCodeResult;
        statusResult.Should().NotBeNull();
        if (statusResult != null)
        {
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }

    [Fact]
    public async Task DeleteSpending_SendCommandToMediator_AndReturnsOk()
    {
        //Arrange
        var id = Guid.NewGuid();
        var userId = _fixture.Create<string>();

        _userInspector.GetAuthenticatedUser().Returns(userId);

        //Act
        var result = await _controller.DeleteSpending(id);

        //Assert
        await _mediator.Received(1).Send(
            Arg.Is<DeleteSpendingCommand>(c => 
                c.Id == id
                && c.UserId == userId
            )
        );

        result.Should().NotBeNull();
        result.Should().BeOfType<OkResult>();
    }
}