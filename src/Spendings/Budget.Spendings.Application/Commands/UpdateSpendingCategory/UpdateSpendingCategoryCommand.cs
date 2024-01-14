using MediatR;

using Budget.Spendings.Domain.Entities;

namespace Budget.Spendings.Application.Commands.UpdateSpendingCategory;

public class UpdateSpendingCategoryCommand : IRequest
{
    public UpdateSpendingCategoryCommand(
        Guid spendingCategoryId,
        string userId,
        string? name,
        Frequency? frequency,
        bool? isPeriodOpened,
        double? amount,
        string? description
    )
    {
        if (spendingCategoryId == Guid.Empty)
            throw new ArgumentException("Spending category id cannot be empty");

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User id cannot be empty");

        SpendingCategoryId = spendingCategoryId;
        UserId = userId;
        Name = name;
        Frequency = frequency;
        IsPeriodOpened = isPeriodOpened;
        Amount = amount;
        Description = description;
    }

    public Guid SpendingCategoryId { get; }

    public string UserId { get; }

    public string? Name { get; }

    public Frequency? Frequency { get; }

    public bool? IsPeriodOpened { get; }

    public double? Amount { get; }

    public string? Description { get; }
}