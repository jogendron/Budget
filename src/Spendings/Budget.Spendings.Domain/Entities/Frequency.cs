using System.Runtime.Serialization;

namespace Budget.Spendings.Domain.Entities;

public enum Frequency
{
    [EnumMember(Value = "Daily")]
    Daily,

    [EnumMember(Value = "Weekly")]
    Weekly,

    [EnumMember(Value = "SemiWeekly")]
    SemiWeekly,

    [EnumMember(Value = "Monthly")]
    Monthly,

    [EnumMember(Value = "Yearly")]
    Yearly
}

internal static class FrequencyExtensions
{
    internal static Frequency ToFrequency(this string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return value.ToLower().Trim() switch 
        {
            "daily" => Frequency.Daily,
            "weekly" => Frequency.Weekly,
            "semiweekly" => Frequency.SemiWeekly,
            "monthly" => Frequency.Monthly,
            "yearly" => Frequency.Yearly,
            _ => throw new ArgumentException($"Cannot parse unknown frequency {value}")
        };
    }
}