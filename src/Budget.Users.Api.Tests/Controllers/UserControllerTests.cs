using System;
using Xunit;
using NSubstitute;
using FluentAssertions;
using AutoFixture;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MediatR;
using Budget.Users.Api.Controllers;
using Budget.Users.Application.Commands.Subscribe;
using Budget.Users.Application.Queries.GetUser;
using Budget.Users.Application.Exceptions;
using Budget.Users.Domain.Model.ReadModel;

namespace Budget.Users.Api.Tests
{
    public class UserControllerTests
    {
        private Fixture fixture;
        private IMediator mediator;
        private UserController controller;

        public UserControllerTests()
        {
            fixture = new Fixture();
            mediator = Substitute.For<IMediator>();
            controller = new UserController(mediator);
        }

        [Fact]
        public void Subscribe_ReturnsStatus201_WhenSusbcriptionWorks()
        {
            //Arrange
            SubscribeCommand command = fixture.Create<SubscribeCommand>();

            //Act
            Task<ActionResult> task = controller.Subscribe(command);
            task.Wait();

            ActionResult response = task.Result;

            //Assert
            mediator.Received(1).Send(Arg.Is(command));
            
            response.Should().NotBeNull();
            response.Should().BeOfType<CreatedAtActionResult>();

            CreatedAtActionResult result = response as CreatedAtActionResult;
            result.StatusCode.Should().Be(StatusCodes.Status201Created);

            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<GetUserRequest>();
            GetUserRequest resultValue = result.Value as GetUserRequest;
            resultValue.UserName.Should().Be(command.UserName);
        }

        [Fact]
        public void Subscribe_ReturnsBadRequest_WhenFormatExceptionOccurs()
        {
            //Arrange
            SubscribeCommand command = fixture.Create<SubscribeCommand>();

            mediator.When(
                m => m.Send(Arg.Is(command))
            ).Do(args => throw new FormatException());

            //Act
            Task<ActionResult> task = controller.Subscribe(command);
            task.Wait();

            ActionResult response = task.Result;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void Subscribe_ReturnsBadRequest_WhenArgumentExceptionOccurs()
        {
            //Arrange
            SubscribeCommand command = fixture.Create<SubscribeCommand>();

            mediator.When(
                m => m.Send(Arg.Is(command))
            ).Do(args => throw new ArgumentException());

            //Act
            Task<ActionResult> task = controller.Subscribe(command);
            task.Wait();

            ActionResult response = task.Result;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void Subscribe_ReturnsBadRequest_WhenArgumentNullExceptionOccurs()
        {
            //Arrange
            SubscribeCommand command = fixture.Create<SubscribeCommand>();

            mediator.When(
                m => m.Send(Arg.Is(command))
            ).Do(args => throw new ArgumentNullException());

            //Act
            Task<ActionResult> task = controller.Subscribe(command);
            task.Wait();

            ActionResult response = task.Result;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void Subscribe_ReturnsConflict_WhenUserAlreadyExists()
        {
            //Arrange
            SubscribeCommand command = fixture.Create<SubscribeCommand>();

            mediator.When(
                m => m.Send(Arg.Is(command))
            ).Do(args => throw new UserAlreadyExistsException());

            //Act
            Task<ActionResult> task = controller.Subscribe(command);
            task.Wait();

            ActionResult response = task.Result;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<StatusCodeResult>();

            StatusCodeResult result = response as StatusCodeResult;
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }

        [Fact]
        public void Subscribe_ReturnsInternalError_WhenOtherExceptionOccurs()
        {
            //Arrange
            SubscribeCommand command = fixture.Create<SubscribeCommand>();

            mediator.When(
                m => m.Send(Arg.Is(command))
            ).Do(args => throw new Exception());

            //Act
            Task<ActionResult> task = controller.Subscribe(command);
            task.Wait();

            ActionResult response = task.Result;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<StatusCodeResult>();

            StatusCodeResult result = response as StatusCodeResult;
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void GetUser_ReturnsOk_WhenUserIsFound()
        {
            //Arrange
            GetUserRequest request = fixture.Create<GetUserRequest>();
            User user = fixture.Create<User>();            

            mediator.Send(Arg.Is(request)).Returns(user);

            //Act
            Task<ActionResult> task = controller.GetUser(request);
            task.Wait();

            ActionResult result = task.Result;
            
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = result as OkObjectResult;

            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeOfType<User>();

            User value = okResult.Value as User;
            value.Should().Be(user);
        }

        [Fact]
        public void GetUser_ReturnsNotFound_WhenUserIsNotFound()
        {
            //Arrange
            GetUserRequest request = fixture.Create<GetUserRequest>();

            //Act
            Task<ActionResult> task = controller.GetUser(request);
            task.Wait();

            ActionResult result = task.Result;
            
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();

            NotFoundResult notFoundResult = result as NotFoundResult;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public void GetUser_ReturnsInternalError_WhenExceptionOccurs()
        {
            //Arrange
            GetUserRequest request = fixture.Create<GetUserRequest>();       

            mediator.When(m => m.Send(Arg.Is(request))).Do(
                args => throw new Exception()
            );

            //Act
            Task<ActionResult> task = controller.GetUser(request);
            task.Wait();

            ActionResult result = task.Result;
            
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();

            StatusCodeResult statusCodeResult = result as StatusCodeResult;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
