namespace Budget.Spendings.Application.Commands.CreateSpending;

public class CreateSpendingResponse
{
    public CreateSpendingResponse()
    {
        Id = Guid.Empty;
    }

    public CreateSpendingResponse(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}