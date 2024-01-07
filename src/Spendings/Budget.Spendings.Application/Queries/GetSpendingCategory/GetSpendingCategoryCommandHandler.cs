using Budget.Spendings.Application.Exceptions;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Repositories;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoryCommandHandler :
    IRequestHandler<GetSpendingCategoryByIdCommand, SpendingCategory?>,
    IRequestHandler<GetSpendingCategoryByUserAndNameCommand, SpendingCategory?>,
    IRequestHandler<GetSpendingCategoriesByUserCommand, IEnumerable<SpendingCategory>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSpendingCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;        
    }

    public async Task<SpendingCategory?> Handle(
        GetSpendingCategoryByIdCommand request, 
        CancellationToken cancellationToken
    )
    {
        var category = await _unitOfWork.SpendingCategories.GetAsync(request.Id);

        if (category != null && category.UserId != request.UserId)
            throw new CategoryBelongsToAnotherUserException();

        return category;
    }

    public async Task<SpendingCategory?> Handle(
        GetSpendingCategoryByUserAndNameCommand request,
        CancellationToken cancellationToken
    )
    {
        return await _unitOfWork.SpendingCategories.GetAsync(request.UserId, request.Name);
    }

    public async Task<IEnumerable<SpendingCategory>> Handle(
        GetSpendingCategoriesByUserCommand request, 
        CancellationToken cancellationToken
    )
    {
        return await _unitOfWork.SpendingCategories.GetAsync(request.UserId);
    }
    
}