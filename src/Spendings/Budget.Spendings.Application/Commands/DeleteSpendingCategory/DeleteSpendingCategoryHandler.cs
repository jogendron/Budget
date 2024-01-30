using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Budget.Spendings.Application.Commands.DeleteSpendingCategory;

public class DeleteSpendingCategoryHandler : IRequestHandler<DeleteSpendingCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSpendingCategoryHandler> _logger;

    public DeleteSpendingCategoryHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteSpendingCategoryHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }    

    public async Task Handle(DeleteSpendingCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting a spending category");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"DeleteSpendingCategoryCommand: {JsonSerializer.Serialize(request)}");

            _unitOfWork.BeginTransaction();

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var category = await  _unitOfWork.SpendingCategories.GetAsync(request.Id);

            if (category == null)
                throw new CategoryDoesNotExistException();

            if (category.UserId != request.UserId)
                throw new CategoryBelongsToAnotherUserException();

            await _unitOfWork.SpendingCategories.DeleteAsync(category);

            _unitOfWork.Commit();
        }
        catch(Exception exception)
        {
            _unitOfWork.Rollback();
            _logger.LogError(exception, "Spending category delete failed");
            throw;
        }
    }
}