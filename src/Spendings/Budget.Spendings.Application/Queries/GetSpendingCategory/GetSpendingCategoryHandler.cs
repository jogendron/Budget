using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoryHandler :
    IRequestHandler<GetSpendingCategoryByIdCommand, SpendingCategory?>,
    IRequestHandler<GetSpendingCategoryByUserAndNameCommand, SpendingCategory?>,
    IRequestHandler<GetSpendingCategoriesByUserCommand, IEnumerable<SpendingCategory>>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<GetSpendingCategoryHandler> _logger;

    public GetSpendingCategoryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetSpendingCategoryHandler> logger
    )
    {
        _unitOfWork = unitOfWork;        
        _logger = logger;
    }

    public async Task<SpendingCategory?> Handle(
        GetSpendingCategoryByIdCommand request, 
        CancellationToken cancellationToken
    )
    {
        SpendingCategory? category = null;
        try
        {
            _logger.LogInformation("Getting a spending category by id");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"GetSpendingCategoryByIdCommand: {JsonSerializer.Serialize(request)}");

            category = await _unitOfWork.SpendingCategories.GetAsync(request.Id);

            if (category != null && category.UserId != request.UserId)
                throw new CategoryBelongsToAnotherUserException();
            }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        return category;
    }

    public async Task<SpendingCategory?> Handle(
        GetSpendingCategoryByUserAndNameCommand request,
        CancellationToken cancellationToken
    )
    {
        SpendingCategory? category = null;

        try
        {
            _logger.LogInformation("Getting a spending category by user and name");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"GetSpendingCategoryByUserAndName: {JsonSerializer.Serialize(request)}");

            category = await _unitOfWork.SpendingCategories.GetAsync(request.UserId, request.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }

        return category;
    }

    public async Task<IEnumerable<SpendingCategory>> Handle(
        GetSpendingCategoriesByUserCommand request, 
        CancellationToken cancellationToken
    )
    {
        IEnumerable<SpendingCategory> categories = new List<SpendingCategory>();

        try
        {
            _logger.LogInformation("Getting spending categories by user");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"GetSpendingCategoriesByUserCommand: {JsonSerializer.Serialize(request)}");

            categories = await _unitOfWork.SpendingCategories.GetAsync(request.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }

        return categories;
    }
    
}