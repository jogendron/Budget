using Budget.Spendings.Domain.WriteModel.Factories;
using Budget.Spendings.Domain.WriteModel.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Budget.Spendings.Application.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryHandler
    : IRequestHandler<CreateSpendingCategoryCommand, CreateSpendingCategoryResponse>
{
    private readonly SpendingCategoryFactory _factory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public CreateSpendingCategoryHandler(
        IUnitOfWork unitOfWork,
        ILogger logger
    )
    {
        _factory = new SpendingCategoryFactory();
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateSpendingCategoryResponse> Handle(
        CreateSpendingCategoryCommand request, 
        CancellationToken cancellationToken
    )
    {
        var response = new CreateSpendingCategoryResponse();

        try
        {
            _unitOfWork.BeginTransaction();

            var category = _factory.Create(
                request.UserId,
                request.Name,
                request.Frequency,
                request.Amount,
                request.Description
            );
            
            response.Id = category.Id;

            await _unitOfWork.SpendingCategories.SaveAsync(category);
            _unitOfWork.Commit();
        }
        catch(Exception exception)
        {
            _unitOfWork.Rollback();
            _logger.LogError(exception, "Spending category creation failed");
            throw;
        }

        return response;
    }
}