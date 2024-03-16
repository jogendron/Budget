using Budget.Sendings.Api.Configuration;
using Budget.Spendings.Api.Models;
using Budget.Spendings.Api.Services;

using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Application.Commands.CreateSpending;
using Budget.Spendings.Application.Commands.UpdateSpending;
using Budget.Spendings.Application.Commands.DeleteSpending;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
using Budget.Spendings.Application.Queries.GetSpending;

namespace Budget.Spendings.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class SpendingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserInspector _userInspector;
    private readonly ILogger<SpendingsController> _logger;
    
    public SpendingsController(
        IMediator mediator,
        IUserInspector userInspector,
        ILogger<SpendingsController> logger
    )
    {
        _mediator = mediator;
        _userInspector = userInspector;
        _logger = logger;        
    }

    private Spending? ConvertSpending(Domain.Entities.Spending? spending)
    {
        return spending != null ?
            new Spending(spending) 
            : null;
    }

    [HttpPost(Name = "CreateSpending")]
    [RequiredScope(ApiScopes.Write)]
    [ProducesResponseType(typeof(Spending), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateSpending(NewSpending spending)
    {
        ActionResult response = new OkResult();
        
        try
        {
            var creation = await _mediator.Send(new CreateSpendingCommand(
                spending.CategoryId,
                _userInspector.GetAuthenticatedUser(),
                spending.Date,
                spending.Amount,
                spending.Description
            ));

            response = CreatedAtRoute(
                "GetSpendingFromId", 
                new { id = creation.Id }, 
                ConvertSpending(await _mediator.Send(
                    new GetSpendingByIdCommand(
                        creation.Id, 
                        _userInspector.GetAuthenticatedUser()
                    )
                ))
            );
        }
        catch (Exception ex) when (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is CategoryDoesNotExistException 
            || ex is CategoryBelongsToAnotherUserException
        )
        {
            _logger.LogWarning(
                "Failed to create spending with category \"{id}\" because of invalid parameters",
                spending.CategoryId
            );

            response = BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occured :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    [HttpGet("{id:guid}", Name = "GetSpendingFromId")]
    [RequiredScope(ApiScopes.Read)]
    [ProducesResponseType(typeof(Spending), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSpending([FromRoute] Guid id)
    {
        ActionResult response = new OkResult();

        try
        {
            var spending = await _mediator.Send(
                new GetSpendingByIdCommand(id, _userInspector.GetAuthenticatedUser())
            );

            if (spending != null)
                response = new OkObjectResult(ConvertSpending(spending));
            else
                response = NotFound();
        }
        catch (Exception ex) when (
            ex is ArgumentException
            || ex is ArgumentNullException 
        )
        {
            _logger.LogWarning(
                "Failed to get spending with id \"{id}\" because of invalid parameters",
                id
            );

            response = BadRequest();
        }
        catch (SpendingBelongsToAnotherUserException)
        {
            _logger.LogWarning(
                "Failed to get spending with id \"{id}\" because it belongs to another user",
                id
            );

            response = NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occured :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    [HttpGet(Name = "GetSpendings")]
    [RequiredScope(ApiScopes.Read)]
    [ProducesResponseType(typeof(IEnumerable<Spending>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSpendings(
        [FromQuery] Guid? categoryId, 
        [FromQuery] DateTime? beginDate, 
        [FromQuery] DateTime? endDate
    )
    {
        ActionResult response = new OkResult();
        
        try
        {
            List<Spending> spendings = new List<Spending>();
            IEnumerable<Domain.Entities.Spending> queryResult;

            if (categoryId.HasValue)
            {
                queryResult = await _mediator.Send(
                    new GetSpendingsByCategoryCommand(
                        _userInspector.GetAuthenticatedUser(),
                        categoryId.Value,
                        beginDate,
                        endDate
                    )
                );
            }
            else
            {
                queryResult = await _mediator.Send(
                    new GetSpendingsByUserCommand(
                        _userInspector.GetAuthenticatedUser(),
                        beginDate,
                        endDate
                    )
                );
            }

            spendings.AddRange(
                queryResult.Select(
                    s => ConvertSpending(s)
                ).Where(s => s != null)!
            );
            
            if (spendings.Any() || ! categoryId.HasValue)
                response = new OkObjectResult(spendings.AsEnumerable());
            else
                response = NotFound();
        }
        catch (Exception ex) when (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is CategoryBelongsToAnotherUserException
        ) 
        {
            _logger.LogWarning(
                "Failed to get queryResult with category id \"{id}\" because it belongs to another user",
                categoryId
            );

            response = BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occured :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    [HttpPatch(Name = "UpdateSpendings")]
    [RequiredScope(ApiScopes.Write)]
    [ProducesResponseType(typeof(IEnumerable<Spending>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateSpending(SpendingUpdate update)
    {
        ActionResult response = new OkResult();

        try
        {
            var command = new UpdateSpendingCommand(
                update.Id,
                _userInspector.GetAuthenticatedUser(),
                update.CategoryId,
                update.Date,
                update.Amount,
                update.Description
            );

            await _mediator.Send(command);
        }
        catch (Exception ex) when (
            ex is ArgumentException |
            ex is ArgumentNullException 
            | ex is CategoryDoesNotExistException
            | ex is CategoryBelongsToAnotherUserException
        )
        {
            _logger.LogWarning(
                "Failed to update spending \"{id}\" because of invalid parameters",
                update.Id
            );

            response = BadRequest();
        }
        catch (Exception ex) when (ex is SpendingBelongsToAnotherUserException || ex is SpendingDoesNotExistException)
        {
            _logger.LogWarning(
                "User has no spending \"{id}\"",
                update.Id
            );

            response = NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occured :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    [HttpDelete("{id:guid}", Name = "DeleteSpending")]
    [RequiredScope(ApiScopes.Write)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSpending([FromRoute] Guid id)
    {
        ActionResult response = new OkResult();

        try
        {
            var command = new DeleteSpendingCommand(
                id, 
                _userInspector.GetAuthenticatedUser()
            );

            await _mediator.Send(command);
        }
        catch (Exception ex) when (
            ex is ArgumentException
            || ex is ArgumentNullException
        )
        {
            _logger.LogWarning(
                "Failed to delete a spending because of invalid parameters"
            );

            response = BadRequest();
        }
        catch (Exception ex) when (ex is SpendingDoesNotExistException || ex is SpendingBelongsToAnotherUserException)
        {
            _logger.LogWarning(
                "One of the spendings was not found"
            );

            response = NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occured :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }
}