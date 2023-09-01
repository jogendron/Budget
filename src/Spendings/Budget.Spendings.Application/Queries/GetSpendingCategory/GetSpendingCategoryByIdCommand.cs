using Budget.Spendings.Domain.Entities;

using MediatR;

namespace Budget.Spendings.Application.Queries.GetSpendingCategory;

public class GetSpendingCategoryByIdCommand : IRequest<SpendingCategory?>
{
    public GetSpendingCategoryByIdCommand()
    {
        Id = Guid.Empty;
    }

    public GetSpendingCategoryByIdCommand(Guid id)
    {
        Id = id;
    }
   
    public Guid Id { get; set; }
}