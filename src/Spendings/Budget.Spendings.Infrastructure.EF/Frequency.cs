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
    public static Frequency ToDbFrequency(this Budget.Spendings.Domain.WriteModel.Entities.Frequency frequency)
    {
        return frequency switch 
        {
            Domain.WriteModel.Entities.Frequency.Daily => Frequency.Daily,
            Domain.WriteModel.Entities.Frequency.Weekly => Frequency.Weekly,
            Domain.WriteModel.Entities.Frequency.SemiWeekly => Frequency.SemiWeekly,
            Domain.WriteModel.Entities.Frequency.Monthly => Frequency.Monthly,
            Domain.WriteModel.Entities.Frequency.Yearly => Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot parse unknown frequency {frequency}")
        };
    }

    public static Domain.ReadModel.Entities.Frequency ToReadModel(this Frequency frequency)
    {
        return frequency switch
        {
            Frequency.Daily => Domain.ReadModel.Entities.Frequency.Daily,
            Frequency.Weekly => Domain.ReadModel.Entities.Frequency.Weekly,
            Frequency.SemiWeekly => Domain.ReadModel.Entities.Frequency.SemiWeekly,
            Frequency.Monthly => Domain.ReadModel.Entities.Frequency.Monthly,
            Frequency.Yearly => Domain.ReadModel.Entities.Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot parse unknown frequency {frequency}")
        };
    }
}