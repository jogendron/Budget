using Budget.Spendings.Domain.Entities;

using MediatR;

namespace Budget.Spendings.Application.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryCommand
    : IRequest<CreateSpendingCategoryResponse>
{

    public CreateSpendingCategoryCommand(
        string userId,
        string name,
        Frequency frequency,
        double amount,
        string description
    )
    {
        UserId = userId;
        Name = name;
        Frequency = frequency;
        Amount = amount;
        Description = description;
    }

    public string UserId { get; }

    public string Name { get; }

    public Frequency Frequency { get; }

    public double Amount { get; }

    public string Description { get; }
}