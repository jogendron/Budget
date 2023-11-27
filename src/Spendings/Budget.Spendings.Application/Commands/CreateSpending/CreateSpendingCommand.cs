using MediatR;

namespace Budget.Spendings.Application.Commands.CreateSpending;

public class CreateSpendingCommand : IRequest<CreateSpendingResponse>
{
    public CreateSpendingCommand(
        Guid categoryId,
        string userId,
        DateTime date,
        double amount,
        string description
    )
    {
        CategoryId = categoryId;
        UserId = userId;
        Date = date;
        Amount = amount;
        Description = description;        
    }

    public Guid CategoryId { get; }

    public string UserId { get; }

    public DateTime Date { get; }

    public double Amount { get; }

    public string Description { get; }
}