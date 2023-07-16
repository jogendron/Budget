namespace Budget.Spendings.Application.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryResponse
{
    public CreateSpendingCategoryResponse()
    {
        Id = Guid.Empty;
    }

    public CreateSpendingCategoryResponse(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; internal set; }
}