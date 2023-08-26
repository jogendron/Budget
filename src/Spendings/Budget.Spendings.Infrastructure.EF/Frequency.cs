namespace Budget.Spendings.Infrastructure.EF;

public enum Frequency
{
    Daily,

    Weekly,

    SemiWeekly,

    Monthly,

    Yearly
}

public static class FrequencyExtensions
{
    public static Frequency ToDbFrequency(this Budget.Spendings.Domain.Entities.Frequency frequency)
    {
        return frequency switch 
        {
            Domain.Entities.Frequency.Daily => Frequency.Daily,
            Domain.Entities.Frequency.Weekly => Frequency.Weekly,
            Domain.Entities.Frequency.SemiWeekly => Frequency.SemiWeekly,
            Domain.Entities.Frequency.Monthly => Frequency.Monthly,
            Domain.Entities.Frequency.Yearly => Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot parse unknown frequency {frequency}")
        };
    }

    public static Domain.Entities.Frequency ToDomainFrequency(this Frequency frequency)
    {
        return frequency switch
        {
            Frequency.Daily => Domain.Entities.Frequency.Daily,
            Frequency.Weekly => Domain.Entities.Frequency.Weekly,
            Frequency.SemiWeekly => Domain.Entities.Frequency.SemiWeekly,
            Frequency.Monthly => Domain.Entities.Frequency.Monthly,
            Frequency.Yearly => Domain.Entities.Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot parse unknown frequency {frequency}")
        };
    }
}