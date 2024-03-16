using MediatR;

namespace Budget.Spendings.Application.Commands.CreateSpending;

public class CreateSpendingCommand : IRequest<CreateSpendingResponse>
{
    public CreateSpendingCommand(
        string userId,
        Guid categoryId,
        DateTime date,
        double amount,
        string description
    )
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category id cannot be empty");

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        UserId = userId;
        CategoryId = categoryId;
        Date = date;
        Amount = amount;
        Description = description;        
    }

    public string UserId { get; }

    public Guid CategoryId { get; }

    public DateTime Date { get; }

    public double Amount { get; }

    public string Description { get; }
}