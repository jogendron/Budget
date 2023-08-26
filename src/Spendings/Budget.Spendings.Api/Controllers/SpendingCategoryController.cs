using Budget.Spendings.Api.Exceptions;
using Budget.Spendings.Api.Models;

using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Application.Queries.GetSpendingCategory;
using Budget.Spendings.Application.Exceptions;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Budget.Spendings.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class SpendingCategoryController : ControllerBase
{
    private readonly ILogger<SpendingCategoryController> _logger;
    private readonly IMediator _mediator;

    public SpendingCategoryController(
        ILogger<SpendingCategoryController> logger,
        IMediator mediator
    )
    {
        _logger = logger;
        _mediator = mediator;
    }

    private string Username 
    {
        get 
        {
            var value = HttpContext.User.Identity?.Name ?? string.Empty;

            if (string.IsNullOrEmpty(value))
                throw new WrongUserException();

            return value;
        }
    }

    private SpendingCategory? ConvertSpendingCategory(Domain.Entities.SpendingCategory? domainCategory)
    {
        SpendingCategory? modelCategory = null;

        if (domainCategory != null)
        {
            //Do not return a category that does not belong to the user
            //Do not let an impostor know if an id exists or not
            if (domainCategory?.UserId == Username)
                modelCategory = new SpendingCategory(domainCategory);
            else
                throw new WrongUserException();
        }
            
        return modelCategory;
    }

    [HttpPost(Name = "CreateSpendingCategory")]
    [ProducesResponseType(typeof(SpendingCategory), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateSpendingCategory(NewSpendingCategory newSpendingCategory)
    {
        ActionResult response;

        try
        {
            var creation = await _mediator.Send(newSpendingCategory.ToCreateCommand(Username));

            var getCategoryByIdCommand = new GetSpendingCategoryByIdCommand(creation.Id);

            response = CreatedAtRoute(
                "GetSpendingCategoryFromId", 
                getCategoryByIdCommand, 
                ConvertSpendingCategory(await _mediator.Send(getCategoryByIdCommand))
            );
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException)
        {
            _logger.LogWarning(
                "Failed to create spending category \"{name}\" because of invalid parameters",
                newSpendingCategory.Name
            );

            response = BadRequest();
        }
        catch (SpendingCategoryAlreadyExistsException)
        {
            _logger.LogWarning(
                "Failed to create spending category \"{name}\" because it already exists",
                newSpendingCategory.Name
            );

            response = Conflict();
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occure :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }


    [HttpGet("{id:guid}", Name = "GetSpendingCategoryFromId")]
    [ProducesResponseType(typeof(SpendingCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSpendingCategoryFromId([FromRoute] Guid id)
    {
        ActionResult response;

        try
        {
            var command = new GetSpendingCategoryByIdCommand(id);
            var category = ConvertSpendingCategory(await _mediator.Send(command));

            if (category != null)
                response = new OkObjectResult(category);
            else
                response = NotFound();
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException)
        {
            response = BadRequest();

            _logger.LogWarning(
                "Failed to get spending category from id\n{exception}",
                ex.ToString()
            );
        }
        catch (WrongUserException)
        {
            response = NotFound();

            _logger.LogWarning(
                "User {user} attempted to get someone else's spending category ({id})",
                Username,
                id
            );
        }
        catch (Exception ex)
        {
            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);

            _logger.LogError(
                "An unexpected exception occured while getting category from id\n{exception}",
                ex.ToString()
            );
        }

        return response;
    }


    [HttpGet(Name = "GetSpendingCategoryFromName")]
    [ProducesResponseType(typeof(SpendingCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSpendingCategoryFromName([FromQuery] string name)
    {
        ActionResult response;

        try
        {
            var command = new GetSpendingCategoryByUserAndNameCommand(
                Username,
                name
            );
            var category = ConvertSpendingCategory(await _mediator.Send(command));

            if (category != null)
                response = new OkObjectResult(category);
            else
                response = NotFound();
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException)
        {
            response = BadRequest();

            _logger.LogWarning(
                "Failed to get spending category from id\n{exception}",
                ex.ToString()
            );
        }
        catch (Exception ex)
        {
            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);

            _logger.LogError(
                "An unexpected exception occured while getting category from id\n{exception}",
                ex.ToString()
            );
        }

        return response;
    }

}