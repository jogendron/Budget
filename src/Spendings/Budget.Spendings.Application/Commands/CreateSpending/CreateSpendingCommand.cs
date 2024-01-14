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
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category id cannot be empty");

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

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