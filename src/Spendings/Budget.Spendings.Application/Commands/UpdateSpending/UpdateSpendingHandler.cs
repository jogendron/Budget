using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Budget.Spendings.Application.Commands.UpdateSpending;

public class UpdateSpendingHandler : IRequestHandler<UpdateSpendingCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<UpdateSpendingHandler> _logger;

    public UpdateSpendingHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateSpendingHandler> logger
    )
    {
        _unitOfWork = unitOfWork;        
        _logger = logger;
    }

    public async Task Handle(UpdateSpendingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating a spending");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"UpdateSpendingCommand: {JsonSerializer.Serialize(request)}");
            
            _unitOfWork.BeginTransaction();

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var spending = await _unitOfWork.Spendings.GetAsync(request.SpendingId);

            if (spending == null)
                throw new SpendingDoesNotExistException();

            var category = await _unitOfWork.SpendingCategories.GetAsync(spending.CategoryId);

            if (category!.UserId != request.UserId)
                throw new SpendingBelongsToAnotherUserException();

            if (request.CategoryId != null)
            {
                var newCategory = await _unitOfWork.SpendingCategories.GetAsync(request.CategoryId.Value);

                if (newCategory == null)
                    throw new CategoryDoesNotExistException();
                else if (newCategory.UserId != request.UserId)
                    throw new CategoryBelongsToAnotherUserException();
            }

            var categoryId = request.CategoryId ?? spending.CategoryId;
            var date = request.Date ?? spending.Date;
            var amount = request.Amount ?? spending.Amount;
            var description = request.Description ?? spending.Description;

            spending.Update(categoryId, date, amount, description);
            await _unitOfWork.Spendings.SaveAsync(spending);
            _unitOfWork.Commit();
        }
        catch(Exception exception)
        {
            _unitOfWork.Rollback();
            _logger.LogError(exception, "Spending update failed");
            throw;
        }
    }
}