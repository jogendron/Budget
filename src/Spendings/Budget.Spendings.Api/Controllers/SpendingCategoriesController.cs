using Budget.Spendings.Api.Models;
using Budget.Spendings.Api.Services;

using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Application.Commands.UpdateSpendingCategory;
using Budget.Spendings.Application.Queries.GetSpendingCategory;
using Budget.Spendings.Application.Exceptions;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
using Budget.Sendings.Api.Configuration;
using Budget.Spendings.Application.Commands.DeleteSpendingCategory;

namespace Budget.Spendings.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class SpendingCategoriesController : ControllerBase
{
    private readonly ILogger<SpendingCategoriesController> _logger;
    private readonly IMediator _mediator;
    private readonly IUserInspector _userInspector;

    public SpendingCategoriesController(
        ILogger<SpendingCategoriesController> logger,
        IMediator mediator,
        IUserInspector userInspector
    )
    {
        _logger = logger;
        _mediator = mediator;
        _userInspector = userInspector;
    }

    private IEnumerable<SpendingCategoryEvent> GetHistoryFrom(Domain.Entities.SpendingCategory? domainCategory)
    {
        var events = new List<SpendingCategoryEvent>();

        if (domainCategory != null)
        {
            events = domainCategory.Changes.Select(c => new SpendingCategoryEvent() {
                    EventId = c.EventId,
                    EventDate = c.EventDate,
                    EventType = c.GetType().ToString().Split('.').Last()
                }).ToList();
        }

        return events;
    }

    [HttpPost(Name = "CreateSpendingCategory")]
    [RequiredScope(ApiScopes.Write)]
    [ProducesResponseType(typeof(SpendingCategory), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateSpendingCategory(NewSpendingCategory newSpendingCategory)
    {
        ActionResult response;

        try
        {
            var command = new CreateSpendingCategoryCommand(
                _userInspector.GetAuthenticatedUser(),
                newSpendingCategory.Name,
                newSpendingCategory.Frequency,
                newSpendingCategory.Amount,
                newSpendingCategory.Description
            );

            var creation = await _mediator.Send(command);

            var getCategoryByIdCommand = new GetSpendingCategoryByIdCommand(creation.Id, _userInspector.GetAuthenticatedUser());
            var category = await _mediator.Send(getCategoryByIdCommand);

            response = CreatedAtRoute(
                "GetSpendingCategoryById", 
                new { id = creation.Id }, 
                new SpendingCategory(category!)
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
            _logger.LogError("An unexpected error occured :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    [HttpGet("{id:guid}", Name = "GetSpendingCategoryById")]
    [RequiredScope(ApiScopes.Read)]
    [ProducesResponseType(typeof(SpendingCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSpendingCategoryFromId([FromRoute] Guid id)
    {
        ActionResult response;

        try
        {
            var command = new GetSpendingCategoryByIdCommand(id, _userInspector.GetAuthenticatedUser());
            var domainCategory = await _mediator.Send(command);

            if (domainCategory != null)
                response = new OkObjectResult(new SpendingCategory(domainCategory));
            else
                response = NotFound();
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
        {
            response = BadRequest();

            _logger.LogWarning(
                "Failed to get spending category from id\n{exception}",
                ex.ToString()
            );
        }
        catch (CategoryBelongsToAnotherUserException)
        {
            response = NotFound();

            _logger.LogWarning(
                "User {user} attempted to get someone else's spending category ({id})",
                _userInspector.GetAuthenticatedUser(),
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

    [HttpGet(Name = "GetSpendingCategories")]
    [RequiredScope(ApiScopes.Read)]
    [ProducesResponseType(typeof(IEnumerable<SpendingCategory>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSpendingCategories([FromQuery] string name = "")
    {
        List<SpendingCategory> categories = new List<SpendingCategory>();
        ActionResult response;

        try
        {
            if (! string.IsNullOrEmpty(name))
            {
                var command = new GetSpendingCategoryByUserAndNameCommand(
                    _userInspector.GetAuthenticatedUser(),
                    name
                );

                var domainCategory = await _mediator.Send(command);
                
                if (domainCategory != null)
                    categories.Add(new SpendingCategory(domainCategory));
            }
            else
            {
                var command = new GetSpendingCategoriesByUserCommand(
                    _userInspector.GetAuthenticatedUser()
                );

                categories.AddRange(
                    (await _mediator.Send(command)).Where(c => c != null).Select(
                        c => new SpendingCategory(c)
                    )
                );
            }

            response = categories.Any() || string.IsNullOrEmpty(name)
                ? new OkObjectResult(categories.AsEnumerable())
                : new NotFoundResult();
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
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

    [HttpGet("{id:guid}/history", Name = "GetSpendingCategoryHistoryFromId")]
    [RequiredScope(ApiScopes.Read)]
    [ProducesResponseType(typeof(SpendingCategoryEvent), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSpendingCategoryHistoryFromId([FromRoute] Guid id)
    {
        ActionResult response;

        try
        {
            var command = new GetSpendingCategoryByIdCommand(id, _userInspector.GetAuthenticatedUser());
            var history = GetHistoryFrom(await _mediator.Send(command));

            if (history != null && history.Any())
                response = new OkObjectResult(history);
            else
                response = NotFound();
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
        {
            response = BadRequest();

            _logger.LogWarning(
                "Failed to get spending category history from id\n{exception}",
                ex.ToString()
            );
        }
        catch (CategoryBelongsToAnotherUserException)
        {
            response = NotFound();

            _logger.LogWarning(
                "User {user} attempted to get someone else's spending category history ({id})",
                _userInspector.GetAuthenticatedUser(),
                id
            );
        }
        catch (Exception ex)
        {
            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);

            _logger.LogError(
                "An unexpected exception occured while getting category history from id\n{exception}",
                ex.ToString()
            );
        }

        return response;
    } 

    [HttpPatch(Name = "UpdateSpendingCategory")]
    [RequiredScope(ApiScopes.Read)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateSpendingCategory(SpendingCategoryUpdate update)
    {
        ActionResult response;

        try
        {
            var command = new UpdateSpendingCategoryCommand(
                update.Id,
                _userInspector.GetAuthenticatedUser(),
                update.Name,
                update.Frequency,
                update.IsPeriodOpened,
                update.Amount,
                update.Description
            );

            await _mediator.Send(command);

            response = Ok();
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException)
        {
            _logger.LogWarning(
                "Failed to update spending category \"{name}\" because of invalid parameters",
                update.Name
            );

            response = BadRequest();
        }
        catch (Exception ex) when (ex is CategoryDoesNotExistException || ex is CategoryBelongsToAnotherUserException)
        {
            _logger.LogWarning(
                "User {user} has no category with id {id}",
                _userInspector.GetAuthenticatedUser(),
                update.Id
            );

            response = NotFound();
        }
        catch (SpendingCategoryAlreadyExistsException)
        {
            _logger.LogWarning(
                "Failed to rename spending category to \"{name}\" because it already exists",
                update.Name
            );

            response = Conflict();
        }
        catch(Exception ex)
        {
            _logger.LogError("An unexpected error occured :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    [HttpDelete("{id:guid}", Name = "DeleteSpendingCategory")]
    [RequiredScope(ApiScopes.Write)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSpendingCategory([FromRoute] Guid id)
    {
        ActionResult response = Ok();

        try
        {
            var command = new DeleteSpendingCategoryCommand(
                id, 
                _userInspector.GetAuthenticatedUser()
            );

            await _mediator.Send(command);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
        {
            _logger.LogWarning(
                "Failed to delete spending category \"{id}\" because of invalid parameters",
                id
            );

            response = BadRequest();
        }
        catch (Exception ex) when (ex is CategoryDoesNotExistException || ex is CategoryBelongsToAnotherUserException)
        {
            _logger.LogWarning(
                "User {user} has no category with id {id}",
                _userInspector.GetAuthenticatedUser(),
                id
            );

            response = NotFound();
        }
        catch(Exception ex)
        {
            _logger.LogError("An unexpected error occured :\n{exception}", ex.ToString());

            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return response;
    }
}