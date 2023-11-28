using Budget.Sendings.Api.Configuration;
using Budget.Spendings.Api.Models;
using Budget.Spendings.Api.Services;

using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Application.Commands.CreateSpending;

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
    [ProducesResponseType(typeof(SpendingCategory), StatusCodes.Status201Created)]
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
        catch (Exception ex) when (ex is CategoryDoesNotExistException || ex is CategoryBelongsToAnotherUserException)
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
    [ProducesResponseType(typeof(SpendingCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
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
                response = new OkObjectResult(spending);
        }
        catch (Exception ex) when (ex is SpendingBelongsToAnotherUserException)
        {
            _logger.LogWarning(
                "Failed to get spending with id \"{id}\" because it belongs to another user",
                id
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
}