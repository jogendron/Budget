namespace Budget.Spendings.Domain.Events;

public class Period
{
    public Period()
    {
        BeginDate = DateTime.MinValue;
        EndDate = null;
    }

    public Period(DateTime beginDate)
    {
        BeginDate = beginDate;
    }

    public Period(DateTime beginDate, DateTime? endDate)
    {
        BeginDate = beginDate;
        EndDate = endDate;
    }

    public DateTime BeginDate { get; set; }

    public DateTime? EndDate { get; set; }
}

public static class PeriodExtensions
{
    public static Period ToEventPeriod(this WriteModel.Entities.Period period)
    {
        return new Period(period.BeginDate, period.EndDate);
    }

    public static WriteModel.Entities.Period ToWriteModelPeriod(this Period period)
    {
        return period.EndDate == null ?
            new WriteModel.Entities.Period(period.BeginDate)
            : new WriteModel.Entities.Period(period.BeginDate, period.EndDate.Value);
    }
}