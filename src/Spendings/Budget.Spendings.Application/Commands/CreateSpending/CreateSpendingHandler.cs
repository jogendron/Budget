using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Budget.Spendings.Application.Commands.CreateSpending;

public class CreateSpendingHandler : IRequestHandler<CreateSpendingCommand, CreateSpendingResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SpendingFactory _factory;
    private readonly ILogger<CreateSpendingHandler> _logger;

    public CreateSpendingHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateSpendingHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _factory = new SpendingFactory();        
        _logger = logger;
    }

    public async Task<CreateSpendingResponse> Handle(CreateSpendingCommand request, CancellationToken cancellationToken)
    {
        CreateSpendingResponse response;

        try
        {
            _unitOfWork.BeginTransaction();

            var category = await _unitOfWork.SpendingCategories.GetAsync(request.CategoryId);

            if (category == null)
                throw new CategoryDoesNotExistException();

            if (category.UserId != request.UserId)
                throw new CategoryBelongsToAnotherUserException();

            var spending = _factory.Create(
                request.CategoryId,
                request.Date,
                request.Amount,
                request.Description
            );
            
            await _unitOfWork.Spendings.SaveAsync(spending);
            _unitOfWork.Commit();

            response = new CreateSpendingResponse(spending.Id);
        }
        catch(Exception exception)
        {
            _unitOfWork.Rollback();
            _logger.LogError(exception, "Spending creation failed");
            throw;
        }

        return response;
    }
}