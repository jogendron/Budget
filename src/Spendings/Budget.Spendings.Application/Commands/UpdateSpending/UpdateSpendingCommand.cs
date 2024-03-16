using MediatR;

namespace Budget.Spendings.Application.Commands.UpdateSpending;

public class UpdateSpendingCommand : IRequest
{
    public UpdateSpendingCommand(
        string userId,
        Guid spendingId,
        Guid? categoryId,
        DateTime? date,
        double? amount,
        string? description
    )
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        if (spendingId == Guid.Empty)
            throw new ArgumentException("Spending id cannot be empty");

        UserId = userId;
        SpendingId = spendingId;
        CategoryId = categoryId;
        Date = date;
        Amount = amount;
        Description = description;
    }

    public string UserId { get; }

    public Guid SpendingId { get; }

    public Guid? CategoryId { get; }

    public DateTime? Date { get; }

    public double? Amount { get; }

    public string? Description { get; }

}