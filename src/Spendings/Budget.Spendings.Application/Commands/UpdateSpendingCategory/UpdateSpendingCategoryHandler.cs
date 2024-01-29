using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Budget.Spendings.Application.Commands.UpdateSpendingCategory;

public class UpdateSpendingCategoryHandler : IRequestHandler<UpdateSpendingCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<UpdateSpendingCategoryHandler> _logger;

    public UpdateSpendingCategoryHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateSpendingCategoryHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;        
    }

    public async Task Handle(UpdateSpendingCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating a spending category");

            if (request == null)
                throw new ArgumentNullException(nameof(request));
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"UpdateSpendingCategoryCommand: {JsonSerializer.Serialize(request)}");

            var category = await _unitOfWork.SpendingCategories.GetAsync(request.SpendingCategoryId);

            if (category == null)
                throw new CategoryDoesNotExistException($"Spending category {request.SpendingCategoryId} does not exists.");

            await ValidateRequest(request, category);

            _unitOfWork.BeginTransaction();

            string name = request.Name ?? category.Name;
            Frequency frequency = request.Frequency ?? category.Frequency;
            double amount = request.Amount ?? category.Amount;
            string description = request.Description ?? category.Description;

            category.Update(name, frequency, amount, description);

            if (request.IsPeriodOpened.HasValue)
            {
                if (request.IsPeriodOpened.Value && category.Period.EndDate != null)
                    category.OpenPeriod();
                else if (! request.IsPeriodOpened.Value && category.Period.EndDate == null)
                    category.ClosePeriod();
            }
            
            await _unitOfWork.SpendingCategories.SaveAsync(category);
            
            _unitOfWork.Commit();
        }
        catch(Exception exception)
        {
            _unitOfWork.Rollback();
            _logger.LogError(exception, "Spending category update failed");
            throw;
        }
    }

    private async Task ValidateRequest(UpdateSpendingCategoryCommand request, SpendingCategory category)
    {
        if (category.UserId != request.UserId)
            throw new CategoryBelongsToAnotherUserException();

        if (request.Name != null && request.Name != category.Name)
        {
            var categoryWithNewName = await _unitOfWork.SpendingCategories.GetAsync(request.UserId, request.Name);

            if (categoryWithNewName != null)
                throw new SpendingCategoryAlreadyExistsException();
        }
    }
}