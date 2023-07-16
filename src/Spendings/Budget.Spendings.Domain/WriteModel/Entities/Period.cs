namespace Budget.Spendings.Domain.WriteModel.Entities;

public class Period
{
    private DateTime beginDate;
    private DateTime? endDate;

    public Period(DateTime beginDate)
    {
        BeginDate = beginDate;
        EndDate = null;
    }

    public Period(DateTime beginDate, DateTime endDate)
    {
        BeginDate = beginDate;
        EndDate = endDate;
    }

    public DateTime BeginDate 
    { 
        get
        {
            return beginDate;
        }
        private set
        {
            if (EndDate != null && EndDate.HasValue && EndDate < value)
                throw new ArgumentException("Begin date cannot be after end date.");

            beginDate = value;
        }
    }

    public DateTime? EndDate 
    { 
        get
        {
            return endDate;
        }
        private set
        {
            if (value != null && value < BeginDate)
                throw new ArgumentException("End date cannot be before begin date.");

            endDate = value;
        }
    }

    internal void Close(DateTime endDate)
    {
        if (EndDate != null || EndDate.HasValue)
            throw new InvalidOperationException("Period is already closed.");

        EndDate = endDate;
    }

    internal void Open()
    {
        if (EndDate == null || ! EndDate.HasValue)
            throw new InvalidOperationException("Period is already opened.");

        EndDate = null;
    }
}