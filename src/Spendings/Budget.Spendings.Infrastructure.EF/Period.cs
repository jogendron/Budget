namespace Budget.Spendings.Infrastructure.EF;

public class Period
{
    public Period()
    {
        BeginDate = DateTime.MinValue;
    }

    public Period(DateTime beginDate, DateTime? endDate)
    {
        BeginDate = beginDate;
        EndDate = endDate;
    }

    public DateTime BeginDate { get; set; }

    public DateTime? EndDate { get; set; }
}