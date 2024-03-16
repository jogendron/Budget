using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Budget.Spendings.Application.Queries.GetSpending;

public class GetSpendingHandler 
    :   IRequestHandler<GetSpendingByIdCommand, Spending?>,
        IRequestHandler<GetSpendingsByCategoryCommand, IEnumerable<Spending>>,
        IRequestHandler<GetSpendingsByUserCommand, IEnumerable<Spending>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetSpendingHandler> _logger;

    public GetSpendingHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetSpendingHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Spending?> Handle(GetSpendingByIdCommand request, CancellationToken cancellationToken)
    {
        Spending? response = null;

        try
        {
            _logger.LogInformation("Getting a spending by id");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"GetSpendingByIdCommand: {JsonSerializer.Serialize(request)}");

            var spending = await _unitOfWork.Spendings.GetAsync(request.Id);

            if (spending != null)
            {
                var category = await _unitOfWork.SpendingCategories.GetAsync(spending.CategoryId);

                if (category?.UserId != request.UserId)
                    throw new SpendingBelongsToAnotherUserException();

                response = spending;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        
        return response;
    }

    public async Task<IEnumerable<Spending>> Handle(GetSpendingsByCategoryCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<Spending> response = new List<Spending>();

        try
        {
            _logger.LogInformation("Getting spendings by category");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"GetSpendingsByCategoryCommand: {JsonSerializer.Serialize(request)}");

            var category = await _unitOfWork.SpendingCategories.GetAsync(request.CategoryId);

            if (category?.UserId != request.UserId)
                throw new CategoryBelongsToAnotherUserException();

            response = await _unitOfWork.Spendings.GetAsync(
                request.CategoryId, 
                request.BeginDate, 
                request.EndDate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        
        return response;
    }

    public async Task<IEnumerable<Spending>> Handle(GetSpendingsByUserCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<Spending> response = new List<Spending>();

        try
        {
            _logger.LogInformation("Getting spendings by user");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"GetSpendingsByUserCommand: {JsonSerializer.Serialize(request)}");

            response = await _unitOfWork.Spendings.GetAsync(
                request.UserId,
                request.BeginDate,
                request.EndDate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }

        return response;
    }
}