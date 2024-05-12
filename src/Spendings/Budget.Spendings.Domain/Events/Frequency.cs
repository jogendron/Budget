
namespace Budget.Spendings.Domain.Events;

public enum Frequency
{
    Daily,

    Weekly,

    BiWeekly,

    Monthly,

    Yearly
}

public static class FrequencyExtensions
{
    public static Frequency ToEventFrequency(this Entities.Frequency frequency)
    {
        return frequency switch 
        {
            Entities.Frequency.Daily => Frequency.Daily,
            Entities.Frequency.Weekly => Frequency.Weekly,
            Entities.Frequency.BiWeekly => Frequency.BiWeekly,
            Entities.Frequency.Monthly => Frequency.Monthly,
            Entities.Frequency.Yearly => Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot convert unknown frequency {frequency}")
        };
    }

    public static Entities.Frequency ToDomainFrequency(this Frequency frequency)
    {
        return frequency switch
        {
            Frequency.Daily => Entities.Frequency.Daily,
            Frequency.Weekly => Entities.Frequency.Weekly,
            Frequency.BiWeekly => Entities.Frequency.BiWeekly,
            Frequency.Monthly => Entities.Frequency.Monthly,
            Frequency.Yearly => Entities.Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot convert unknown frequency {frequency}")
        };
    }

}