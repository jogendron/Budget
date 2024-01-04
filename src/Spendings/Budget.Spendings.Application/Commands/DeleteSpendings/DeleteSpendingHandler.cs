using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Budget.Spendings.Application.Commands.DeleteSpendings;

public class DeleteSpendingsHandler : IRequestHandler<DeleteSpendingsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSpendingsHandler> _logger;

    public DeleteSpendingsHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteSpendingsHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteSpendingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await VerifySpendingsExistAndBelongToUser(request);

            await Task.WhenAll(
                request.SpendingIds.Select(
                    id => _unitOfWork.Spendings.DeleteAsync(id)
                )
            );

            _unitOfWork.Commit();
        }
        catch(Exception exception)
        {
            _unitOfWork.Rollback();
            _logger.LogError(exception, "Spending delete failed");
            throw;
        }
    }

    private async Task VerifySpendingsExistAndBelongToUser(DeleteSpendingsCommand request)
    {
        var spendings = (await Task.WhenAll(
                request.SpendingIds.Select(id => 
                    _unitOfWork.Spendings.GetAsync(id)
                )
            )).Where(s => s != null);

            if (! request.SpendingIds.All(id => spendings.Any(s => s!.Id == id)))
                throw new SpendingDoesNotExistException();

            var categories = await Task.WhenAll(
                spendings.Select(
                    s => _unitOfWork.SpendingCategories.GetAsync(s!.CategoryId)
                )
            );

            if (categories.Any(c => c!.UserId != request.UserId))
                throw new SpendingBelongsToAnotherUserException();
    }
}