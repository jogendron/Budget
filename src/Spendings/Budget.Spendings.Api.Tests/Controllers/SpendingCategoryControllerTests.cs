using Budget.Spendings.Api.Controllers;
using Budget.Spendings.Api.Models;
using Budget.Spendings.Api.Services;
using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Application.Queries.GetSpendingCategory;
using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Events;
using Budget.Spendings.Domain.Factories;
using Budget.EventSourcing.Events;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MediatR;

using FluentAssertions;
using NSubstitute;
using AutoFixture;

namespace Budget.Spendings.Api.Tests.Controllers;

public class SpendingCategoryControllerTests
{
    private Fixture _fixture;

    private ILogger<SpendingCategoryController> _logger;
    private IMediator _mediator;
    private IUserInspector _userInspector;
    private SpendingCategoryController _controller;

    public SpendingCategoryControllerTests()
    {
        _fixture = new Fixture();

        _logger = Substitute.For<ILogger<SpendingCategoryController>>();
        _mediator = Substitute.For<IMediator>();
        _userInspector = Substitute.For<IUserInspector>();

        _userInspector.GetAuthenticatedUser().Returns(_fixture.Create<string>());

        _controller = new SpendingCategoryController(_logger, _mediator, _userInspector); 
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtRoute_WithCategory()
    {
        //Arrange
        var newCategory = _fixture.Create<NewSpendingCategory>();
        var id = Guid.NewGuid();
        var domainCategory = new SpendingCategoryFactory().Load(
            id, 
            new List<Event>() {
                _fixture.Build<SpendingCategoryCreated>().With(
                    c => c.AggregateId, id
                ).With(
                    c => c.UserId, _userInspector.GetAuthenticatedUser()
                ).With(
                    c => c.Period, new Domain.Events.Period(DateTime.MinValue)
                ).Create()
            }
        );

        _mediator.Send(Arg.Any<CreateSpendingCategoryCommand>()).Returns(
            new CreateSpendingCategoryResponse(id)
        );

        _mediator.Send(Arg.Is<GetSpendingCategoryByIdCommand>(
                c => c.Id == id
        )).Returns(domainCategory);

        //Act
        var response = await _controller.CreateSpendingCategory(newCategory);

        //Assert
        await _mediator.Received(1).Send(
            Arg.Is<CreateSpendingCategoryCommand>(
                c => c.UserId == _userInspector.GetAuthenticatedUser()
                && c.Name == newCategory.Name
                && c.Amount == newCategory.Amount
                && c.Description == newCategory.Description
            )
        );

        await _mediator.Received(1).Send(
            Arg.Is<GetSpendingCategoryByIdCommand>(
                c => c.Id == id
            )
        );

        response.Should().NotBeNull();
        response.Should().BeOfType<CreatedAtRouteResult>();

        var result = response as CreatedAtRouteResult;
        if (result != null)
        {
            result.RouteName.Should().Be("GetSpendingCategoryFromId");
            result.RouteValues.Should().NotBeNull();
            result.RouteValues!.Count.Should().Be(1);

            result.RouteValues.Count().Should().Be(1);
            result.RouteValues.Values.First().Should().Be(id);

            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<SpendingCategory>();

            var resultCategory = result.Value as SpendingCategory;
            if (resultCategory != null)
            {
                resultCategory.Id.Should().Be(id);
                resultCategory.Name.Should().Be(domainCategory.Name);
                resultCategory.Period.BeginDate.Should().Be(domainCategory.Period.BeginDate);
                resultCategory.Period.EndDate.Should().Be(domainCategory.Period.EndDate);
                resultCategory.Amount.Should().Be(domainCategory.Amount);
                resultCategory.Description.Should().Be(domainCategory.Description);
            }
        }
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenArgumentExceptionOccurs()
    {
        //Arrange
        var newCategory = _fixture.Create<NewSpendingCategory>();

        _mediator.When(m => m.Send(Arg.Any<CreateSpendingCategoryCommand>())).Do(
            callinfo => throw new ArgumentException()
        );

        //Act
        var result = await _controller.CreateSpendingCategory(newCategory);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenArgumentNullExceptionOccurs()
    {
        //Arrange
        var newCategory = _fixture.Create<NewSpendingCategory>();

        _mediator.When(m => m.Send(Arg.Any<CreateSpendingCategoryCommand>())).Do(
            callinfo => throw new ArgumentNullException()
        );

        //Act
        var result = await _controller.CreateSpendingCategory(newCategory);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenInvalidOperationExceptionOccurs()
    {
        //Arrange
        var newCategory = _fixture.Create<NewSpendingCategory>();

        _mediator.When(m => m.Send(Arg.Any<CreateSpendingCategoryCommand>())).Do(
            callinfo => throw new InvalidOperationException()
        );

        //Act
        var result = await _controller.CreateSpendingCategory(newCategory);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenSpendingCategoryAlreadyExists()
    {
        //Arrange
        var newCategory = _fixture.Create<NewSpendingCategory>();

        _mediator.When(m => m.Send(Arg.Any<CreateSpendingCategoryCommand>())).Do(
            callinfo => throw new SpendingCategoryAlreadyExistsException()
        );

        //Act
        var result = await _controller.CreateSpendingCategory(newCategory);

        //Assert
        result.Should().BeOfType<ConflictResult>();
    }

    [Fact]
    public async Task Create_ReturnsInternalError_WhenUnexpectedErrorOccurs()
    {
        //Arrange
        var newCategory = _fixture.Create<NewSpendingCategory>();

        _mediator.When(m => m.Send(Arg.Any<CreateSpendingCategoryCommand>())).Do(
            callinfo => throw new DivideByZeroException()
        );

        //Act
        var result = await _controller.CreateSpendingCategory(newCategory);

        //Assert
        result.Should().BeOfType<StatusCodeResult>();
        
        var statusResult = result as StatusCodeResult;
        if (statusResult != null)
        {
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }

    [Fact]
    public async Task GetSpendingCategoryFromId_ReturnsOkObjectResult_WhenCategoryExists()
    {
        //Arrange
        var id = Guid.NewGuid();
        var domainCategory = new SpendingCategoryFactory().Load(
            id, 
            new List<Event>() {
                _fixture.Build<SpendingCategoryCreated>().With(
                    c => c.AggregateId, id
                ).With(
                    c => c.UserId, _userInspector.GetAuthenticatedUser()
                ).With(
                    c => c.Period, new Domain.Events.Period(DateTime.MinValue)
                ).Create()
            }
        );

        _mediator.Send(Arg.Is<GetSpendingCategoryByIdCommand>(c => c.Id == id)).Returns(domainCategory);

        //Act
        var result = await _controller.GetSpendingCategoryFromId(id);

        //Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        if (okResult != null)
        {
            var value = okResult.Value as SpendingCategory;
            
            value.Should().NotBeNull();
            if (value != null)
            {
                value.Name.Should().Be(domainCategory.Name);
                value.Frequency.Should().Be(domainCategory.Frequency);
                value.Amount.Should().Be(domainCategory.Amount);
                value.Description.Should().Be(domainCategory.Description);
            }
        }
    }

    [Fact]
    public async Task GetSpendingCategoryFromId_ReturnsNotFound_WhenCategoryIsNotFound()
    {
        //Arrange

        //Act
        var result = await _controller.GetSpendingCategoryFromId(Guid.NewGuid());

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSpendingCategoryFromId_ReturnsBadRequest_WhenArgumentExceptionOccurs()
    {
        //Arrange
        _mediator.When(m => m.Send(Arg.Any<GetSpendingCategoryByIdCommand>())).Do(
            callinfo => throw new ArgumentException()
        );

        //Act
        var result = await _controller.GetSpendingCategoryFromId(Guid.NewGuid());

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task GetSpendingCategoryFromId_ReturnsBadRequest_WhenArgumentNullExceptionOccurs()
    {
        //Arrange
        _mediator.When(m => m.Send(Arg.Any<GetSpendingCategoryByIdCommand>())).Do(
            callinfo => throw new ArgumentNullException()
        );

        //Act
        var result = await _controller.GetSpendingCategoryFromId(Guid.NewGuid());

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task GetSpendingCategoryFromId_ReturnsNotFound_WhenCategoryDoesNotBelongToUser()
    {
        //Arrange
        var id = Guid.NewGuid();
        var domainCategory = new SpendingCategoryFactory().Load(
            id, 
            new List<Event>() {
                _fixture.Build<SpendingCategoryCreated>().With(
                    c => c.AggregateId, id
                ).With(
                    c => c.Period, new Domain.Events.Period(DateTime.MinValue)
                ).Create()
            }
        );

        _mediator.Send(Arg.Is<GetSpendingCategoryByIdCommand>(c => c.Id == id)).Returns(domainCategory);

        //Act
        var result = await _controller.GetSpendingCategoryFromId(id);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }   

    [Fact]
    public async Task GetSpendingCategoryFromId_ReturnsInternalError_WhenUnexpectedExceptionOccurs()
    {
        //Arrange
        _mediator.When(m => m.Send(Arg.Any<GetSpendingCategoryByIdCommand>())).Do(
            callInfo => throw new DivideByZeroException()
        );

        //Act
        var result = await _controller.GetSpendingCategoryFromId(Guid.NewGuid());

        //Assert
        result.Should().BeOfType<StatusCodeResult>();

        var statusResult = result as StatusCodeResult;
        statusResult.Should().NotBeNull();
        
        if (statusResult != null)
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetSpendingCategories_GetsCategoryByUserAndName_WhenNameIsProvided()
    {
        //Arrange
        var id = Guid.NewGuid();
        var name = _fixture.Create<string>();

        var domainCategory = new SpendingCategoryFactory().Load(
            id, 
            new List<Event>() {
                _fixture.Build<SpendingCategoryCreated>().With(
                    c => c.AggregateId, id
                ).With(
                    c => c.UserId, _userInspector.GetAuthenticatedUser()
                ).With(
                    c => c.Name, name
                ).With(
                    c => c.Period, new Domain.Events.Period(DateTime.MinValue)
                ).Create()
            }
        );

        _mediator.Send(Arg.Is<GetSpendingCategoryByUserAndNameCommand>(
                c => 
                c.UserId == _userInspector.GetAuthenticatedUser() 
                && c.Name == name
            )
        ).Returns(domainCategory);

        //Act
        var result = await _controller.GetSpendingCategories(name);

        //Assert
        result.Should().BeOfType<OkObjectResult>();

        var objectResult = result as OkObjectResult;
        objectResult.Should().NotBeNull();
        if (objectResult != null)
        {
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.Should().BeAssignableTo<IEnumerable<SpendingCategory>>();

            var receivedCategories = objectResult.Value as IEnumerable<SpendingCategory>;
            if (receivedCategories != null)
            {
                receivedCategories.Count().Should().Be(1);
                var firstCategory = receivedCategories.First();

                firstCategory.Should().NotBeNull();
                firstCategory.Name.Should().Be(domainCategory.Name);
                firstCategory.Frequency.Should().Be(domainCategory.Frequency);
                firstCategory.Amount.Should().Be(domainCategory.Amount);
                firstCategory.Description.Should().Be(domainCategory.Description);
            }
        }
    }

    [Fact]
    public async Task GetSpendingCategories_GetsCategoryByUserOnly_WhenNoNameIsProvided()
    {
        //Arrange
        var id = Guid.NewGuid();
        var factory = new SpendingCategoryFactory();

        var domainCategories = new [] {
            factory.Load(
                id, 
                new List<Event>() {
                    _fixture.Build<SpendingCategoryCreated>().With(
                        c => c.AggregateId, id
                    ).With(
                        c => c.UserId, _userInspector.GetAuthenticatedUser()
                    ).With(
                        c => c.Period, new Domain.Events.Period(DateTime.MinValue)
                    ).Create()
                }
            ),
            factory.Load(
                id, 
                new List<Event>() {
                    _fixture.Build<SpendingCategoryCreated>().With(
                        c => c.AggregateId, id
                    ).With(
                        c => c.UserId, _userInspector.GetAuthenticatedUser()
                    ).With(
                        c => c.Period, new Domain.Events.Period(DateTime.MinValue)
                    ).Create()
                }
            )
        };

        _mediator.Send(Arg.Is<GetSpendingCategoriesByUserCommand>(
                c => 
                c.UserId == _userInspector.GetAuthenticatedUser()
            )
        ).Returns(domainCategories);

        //Act
        var result = await _controller.GetSpendingCategories();

        //Assert
        result.Should().BeOfType<OkObjectResult>();

        var objectResult = result as OkObjectResult;
        objectResult.Should().NotBeNull();
        if (objectResult != null)
        {
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.Should().BeAssignableTo<IEnumerable<SpendingCategory>>();

            var receivedCategories = objectResult.Value as IEnumerable<SpendingCategory>;
            if (receivedCategories != null)
            {
                receivedCategories.Count().Should().Be(2);

                for (int i = 0; i < domainCategories.Count(); i++)
                {
                    var source = domainCategories.ElementAt(i);
                    var destination = receivedCategories.ElementAt(i);

                    source.Should().NotBeNull();
                    source.Name.Should().Be(destination.Name);
                    source.Frequency.Should().Be(destination.Frequency);
                    source.Amount.Should().Be(destination.Amount);
                    source.Description.Should().Be(destination.Description);
                }
            }
        }
    }

    [Fact]
    public async Task GetSpendingCategories_ReturnsBadRequest_WhenArgumentExceptionOccurs()
    {
        //Arrange
        _mediator.When(m => m.Send(Arg.Any<GetSpendingCategoriesByUserCommand>())).Do(
            c => throw new ArgumentException()
        );

        //Act
        var result = await _controller.GetSpendingCategories();

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task GetSpendingCategories_ReturnsBadRequest_WhenArgumentNullExceptionOccurs()
    {
        //Arrange
        _mediator.When(m => m.Send(Arg.Any<GetSpendingCategoriesByUserCommand>())).Do(
            c => throw new ArgumentNullException()
        );

        //Act
        var result = await _controller.GetSpendingCategories();

        //Assert
        result.Should().BeOfType<BadRequestResult>();   
    }

    [Fact]
    public async Task GetSpendingCategories_ReturnsInternalError_WhenUnexpectedExceptionOccurs()
    {
        //Arrange
        _mediator.When(m => m.Send(Arg.Any<GetSpendingCategoriesByUserCommand>())).Do(
            c => throw new DivideByZeroException()
        );

        //Act
        var result = await _controller.GetSpendingCategories();

        //Assert
        result.Should().BeOfType<StatusCodeResult>();

        var statusResult = result as StatusCodeResult;
        statusResult.Should().NotBeNull();
        
        if (statusResult != null)
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}