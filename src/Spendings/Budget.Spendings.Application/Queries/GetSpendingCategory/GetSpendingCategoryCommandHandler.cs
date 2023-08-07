using Budget.Spendings.Domain.ReadModel.Entities;
using Budget.Spendings.Domain.ReadModel.Repositories;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoryCommandHandler :
    IRequestHandler<GetSpendingCategoryByIdCommand, SpendingCategory?>,
    IRequestHandler<GetSpendingCategoryByUserAndNameCommand, SpendingCategory?>,
    IRequestHandler<GetSpendingCategoriesByUserCommand, IEnumerable<SpendingCategory>>
{
    private readonly ISpendingCategoryRepository _repository;

    public GetSpendingCategoryCommandHandler(ISpendingCategoryRepository repository)
    {
        _repository = repository;        
    }

    public async Task<SpendingCategory?> Handle(
        GetSpendingCategoryByIdCommand request, 
        CancellationToken cancellationToken
    )
    {
        return await _repository.Get(request.Id);
    }

    public async Task<SpendingCategory?> Handle(
        GetSpendingCategoryByUserAndNameCommand request,
        CancellationToken cancellationToken
    )
    {
        return await _repository.Get(request.UserId, request.Name);
    }

    public async Task<IEnumerable<SpendingCategory>> Handle(GetSpendingCategoriesByUserCommand request, CancellationToken cancellationToken)
    {
        return await _repository.Get(request.UserId);
    }
}