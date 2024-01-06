using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Budget.Spendings.Application.Commands.DeleteSpending;

public class DeleteSpendingHandler : IRequestHandler<DeleteSpendingCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSpendingHandler> _logger;

    public DeleteSpendingHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteSpendingHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteSpendingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await VerifySpendingsExistAndBelongToUser(request);

            await _unitOfWork.Spendings.DeleteAsync(request.Id);

            _unitOfWork.Commit();
        }
        catch(Exception exception)
        {
            _unitOfWork.Rollback();
            _logger.LogError(exception, "Spending delete failed");
            throw;
        }
    }

    private async Task VerifySpendingsExistAndBelongToUser(DeleteSpendingCommand request)
    {
        var spending = await _unitOfWork.Spendings.GetAsync(request.Id);

        if (spending == null)
            throw new SpendingDoesNotExistException();

        var category = await _unitOfWork.SpendingCategories.GetAsync(spending.CategoryId);

        if (category!.UserId != request.UserId)
            throw new SpendingBelongsToAnotherUserException();
    }
}