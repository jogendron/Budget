using System;

namespace Budget.EventSourcing.Events
{

    public abstract class Event
    {
        //Constructor for serialization
        protected Event()
        {

        }

        protected Event(Guid id, Guid aggregateId, DateTime date)
        {
            Id = id;
            AggregateId = aggregateId;
            Date = date;
        }

        public Guid Id { get; set;} //Public setter because serialization requires it

        public Guid AggregateId { get; set; } //Public setter because serialization requires it

        public DateTime Date { get; set;} //Public setter because serialization requires it

        internal bool IsBefore(DateTime date)
        {
            return Date <= date;
        }
    }

}