using System.ComponentModel.DataAnnotations;

namespace Budget.Spendings.Api.Models;

public class Period
{
    public Period(Domain.Entities.Period period)
    {
        BeginDate = period.BeginDate;
        EndDate = period.EndDate;
    }

    [Required]
    public DateTime BeginDate { get; set; }

    public DateTime? EndDate { get; set; }
}