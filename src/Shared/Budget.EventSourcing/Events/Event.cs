using System;

namespace Budget.EventSourcing.Events;

public abstract class Event
{
    //Constructor for serialization
    protected Event()
    {

    }

    protected Event(Guid id, Guid aggregateId, DateTime date)
    {
        EventId = id;
        AggregateId = aggregateId;
        EventDate = date;
    }

    public Guid EventId { get; set;} //Public setter because serialization requires it

    public Guid AggregateId { get; set; } //Public setter because serialization requires it

    public DateTime EventDate { get; set;} //Public setter because serialization requires it

    internal bool HadHappenedAt(DateTime date)
    {
        return EventDate <= date;
    }
}