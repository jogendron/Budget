using Budget.Spendings.Api.Exceptions;
using Budget.Spendings.Api.Models;

using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Application.Queries.GetSpendingCategory;
using Budget.Spendings.Application.Exceptions;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
using Budget.Spendings.Api.Services;

namespace Budget.Spendings.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class SpendingCategoryController : ControllerBase
{
    private const string readScope = "Spendings.Read";
    private const string writeScope = "Spendings.Write";

    private readonly ILogger<SpendingCategoryController> _logger;
    private readonly IMediator _mediator;
    private readonly IUserInspector _userInspector;

    public SpendingCategoryController(
        ILogger<SpendingCategoryController> logger,
        IMediator mediator,
        IUserInspector userInspector
    )
    {
        _logger = logger;
        _mediator = mediator;
        _userInspector = userInspector;
    }

    private bool CategoryBelongsToUser(Domain.Entities.SpendingCategory category)
    {
        return category.UserId == _userInspector.GetAuthenticatedUser();
    }

    private SpendingCategory? ConvertSpendingCategory(Domain.Entities.SpendingCategory? domainCategory)
    {
        SpendingCategory? modelCategory = null;

        if (domainCategory != null)
        {
            //Do not return a category that does not belong to the user
            //Do not let an impostor know if an id exists or not
            if (CategoryBelongsToUser(domainCategory))
                modelCategory = new SpendingCategory(domainCategory);
            else
                throw new WrongUserException();
        }
            
        return modelCategory;
    }

    private IEnumerable<SpendingCategoryEvent> GetHistoryFrom(Domain.Entities.SpendingCategory? domainCategory)
    {
        var events = new List<SpendingCategoryEvent>();

        if (domainCategory != null)
        {
            //Do not return a category that does not belong to the user
            //Do not let an impostor know if an id exists or not
            if (CategoryBelongsToUser(domainCategory))
                events = domainCategory.Changes.Select(c => new SpendingCategoryEvent() {
                    EventId = c.EventId,
                    EventDate = c.EventDate,
                    EventType = c.GetType().ToString().Split('.').Last()
                }).ToList();
            else
                throw new WrongUserException();
        }

        return events;
    }

    [HttpPost(Name = "CreateSpendingCategory")]
    [RequiredScope(writeScope)]
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

            var getCategoryByIdCommand = new GetSpendingCategoryByIdCommand(creation.Id);

            response = CreatedAtRoute(
                "GetSpendingCategoryFromId", 
                new { id = creation.Id }, 
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
    [RequiredScope(readScope)]
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
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
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
    [RequiredScope(readScope)]
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

                var category = ConvertSpendingCategory(await _mediator.Send(command));
                
                if (category != null)
                    categories.Add(category);
            }
            else
            {
                var command = new GetSpendingCategoriesByUserCommand(
                    _userInspector.GetAuthenticatedUser()
                );

                categories.AddRange(
                    (await _mediator.Send(command)).Select(
                        c => ConvertSpendingCategory(c)
                    ).Where(c => c != null)!
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
    [RequiredScope(readScope)]
    [ProducesResponseType(typeof(SpendingCategoryEvent), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSpendingCategoryHistoryFromId([FromRoute] Guid id)
    {
        ActionResult response;

        try
        {
            var command = new GetSpendingCategoryByIdCommand(id);
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
        catch (WrongUserException)
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

}