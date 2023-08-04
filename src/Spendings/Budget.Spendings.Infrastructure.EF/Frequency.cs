namespace Budget.Spendings.Infrastructure.EF;

public enum Frequency
{
    Daily,

    Weekly,

    SemiWeekly,

    Monthly,

    Yearly
}

internal static class FrequencyExtensions
{
    internal static Frequency ToDbFrequency(this Budget.Spendings.Domain.WriteModel.Entities.Frequency frequency)
    {
        return frequency switch 
        {
            Budget.Spendings.Domain.WriteModel.Entities.Frequency.Daily => Frequency.Daily,
            Budget.Spendings.Domain.WriteModel.Entities.Frequency.Weekly => Frequency.Weekly,
            Budget.Spendings.Domain.WriteModel.Entities.Frequency.SemiWeekly => Frequency.SemiWeekly,
            Budget.Spendings.Domain.WriteModel.Entities.Frequency.Monthly => Frequency.Monthly,
            Budget.Spendings.Domain.WriteModel.Entities.Frequency.Yearly => Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot parse unknown frequency {frequency}")
        };
    }
}