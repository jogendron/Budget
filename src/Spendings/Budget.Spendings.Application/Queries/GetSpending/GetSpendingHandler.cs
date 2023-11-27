using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Budget.Spendings.Application.Queries.GetSpending;

public class GetSpendingHandler : IRequestHandler<GetSpendingByIdCommand, Spending?>
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
}