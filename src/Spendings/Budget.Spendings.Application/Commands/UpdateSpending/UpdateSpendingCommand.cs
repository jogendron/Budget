using MediatR;

namespace Budget.Spendings.Application.Commands.UpdateSpending;

public class UpdateSpendingCommand : IRequest
{
    public UpdateSpendingCommand(
        Guid spendingId,
        string userId,
        Guid? categoryId,
        DateTime? date,
        double? amount,
        string? description
    )
    {
        if (spendingId == Guid.Empty)
            throw new ArgumentException("Spending id cannot be empty");

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        SpendingId = spendingId;
        UserId = userId;
        CategoryId = categoryId;
        Date = date;
        Amount = amount;
        Description = description;
    }

    public Guid SpendingId { get; }

    public string UserId { get; }

    public Guid? CategoryId { get; }

    public DateTime? Date { get; }

    public double? Amount { get; }

    public string? Description { get; }

}