
namespace Budget.Spendings.Domain.Events;

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
    public static Frequency ToEventFrequency(this WriteModel.Entities.Frequency frequency)
    {
        return frequency switch 
        {
            WriteModel.Entities.Frequency.Daily => Frequency.Daily,
            WriteModel.Entities.Frequency.Weekly => Frequency.Weekly,
            WriteModel.Entities.Frequency.SemiWeekly => Frequency.SemiWeekly,
            WriteModel.Entities.Frequency.Monthly => Frequency.Monthly,
            WriteModel.Entities.Frequency.Yearly => Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot convert unknown frequency {frequency}")
        };
    }

    public static WriteModel.Entities.Frequency ToWriteModelFrequency(this Frequency frequency)
    {
        return frequency switch
        {
            Frequency.Daily => WriteModel.Entities.Frequency.Daily,
            Frequency.Weekly => WriteModel.Entities.Frequency.Weekly,
            Frequency.SemiWeekly => WriteModel.Entities.Frequency.SemiWeekly,
            Frequency.Monthly => WriteModel.Entities.Frequency.Monthly,
            Frequency.Yearly => WriteModel.Entities.Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot convert unknown frequency {frequency}")
        };
    }

}