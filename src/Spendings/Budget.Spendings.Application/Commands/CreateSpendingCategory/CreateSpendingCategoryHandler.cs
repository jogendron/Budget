using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Factories;
using Budget.Spendings.Domain.Repositories;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Budget.Spendings.Application.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryHandler
    : IRequestHandler<CreateSpendingCategoryCommand, CreateSpendingCategoryResponse>
{
    private readonly SpendingCategoryFactory _factory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSpendingCategoryHandler> _logger;

    public CreateSpendingCategoryHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateSpendingCategoryHandler> logger
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
            _logger.LogInformation("Creating a spending category");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"CreateSpendingCategoryCommand: {JsonSerializer.Serialize(request)}");

            _unitOfWork.BeginTransaction();

            var existingCategory = await _unitOfWork.SpendingCategories.GetAsync(
                request.UserId,
                request.Name
            );

            if (existingCategory != null)
                throw new SpendingCategoryAlreadyExistsException();

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