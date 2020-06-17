using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MediatR;

using Budget.Users.Application.Commands.Subscribe;
using Budget.Users.Application.Queries.GetUser;
using Budget.Users.Application.Exceptions;
using Budget.Users.Domain.Model.ReadModel;

namespace Budget.Users.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Subscribe([FromBody] SubscribeCommand subscription)
        {
            ActionResult response = new StatusCodeResult(StatusCodes.Status422UnprocessableEntity);
            
            try
            {
                await mediator.Send(subscription);
                response = CreatedAtAction(nameof(GetUser), new GetUserRequest(subscription.UserName));
            }
            catch (UserAlreadyExistsException)
            {
                response = new StatusCodeResult(StatusCodes.Status409Conflict);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException || ex is ArgumentNullException)
            {
                response = BadRequest();
            }
            catch
            {
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            
            return response;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetUser(GetUserRequest query)
        {
            ActionResult response;

            try
            {
                User user = await mediator.Send(query);

                if (user != null)
                    response = Ok(user);
                else 
                    response = NotFound();
            }
            catch
            {
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return response;
        }
    }
}
