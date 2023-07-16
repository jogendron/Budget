using Budget.Spendings.Domain.WriteModel.Entities;

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

    public string UserId { get; set; }

    public string Name { get; set; }

    public Frequency Frequency { get; set; }

    public double Amount { get; set; }

    public string Description { get; set; }
}