using System;

namespace Budget.EventSourcing.Events
{

    public abstract class Event
    {
        //Constructor for serialization
        protected Event()
        {

        }

        protected Event(Guid aggregateId, Guid eventId, DateTime date)
        {
            AggregateId = aggregateId;
            EventId = eventId;
            Date = date;
        }

        public Guid AggregateId { get; set; } //Public setter because serialization requires it

        public Guid EventId { get; set;} //Public setter because serialization requires it

        public DateTime Date { get; set;} //Public setter because serialization requires it

        internal bool IsBefore(DateTime date)
        {
            return Date <= date;
        }
    }

}