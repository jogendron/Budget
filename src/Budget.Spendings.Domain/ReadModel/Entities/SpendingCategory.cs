namespace Budget.Spendings.Domain.ReadModel.Entities;

public class SpendingCategory : IEquatable<SpendingCategory>
{
    public SpendingCategory(
        Guid id,
        string userId,
        string name,
        DateTime createdOn,
        DateTime? modifiedOn,
        DateTime? closedOn,
        Frequency frequency,
        double amount,
        string description
    )
    {
        Id = id;
        UserId = userId;
        Name = name;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
        ClosedOn = closedOn;
        Frequency = frequency;
        Amount = amount;
        Description = description;
    }

    public Guid Id { get; }

    public string UserId { get; }

    public string Name { get; }

    public DateTime CreatedOn { get; }

    public DateTime? ModifiedOn { get; }

    public DateTime? ClosedOn { get; }

    public Frequency Frequency { get; }

    public double Amount { get; }

    public string Description { get; }

    public bool Equals(SpendingCategory? other)
    {
        bool areEquals = false;

        if (other != null)
        {
            areEquals = 
                (Id == other.Id)
                && (UserId == other.UserId)
                && (Name == other.Name)
                && (CreatedOn == other.CreatedOn)
                && (ModifiedOn == other.ModifiedOn)
                && (ClosedOn == other.ClosedOn)
                && (Frequency == other.Frequency)
                && (Amount == other.Amount)
                && (Description == other.Description);
        }

        return areEquals;
    }
}