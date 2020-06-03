using System;

namespace Budget.EventSourcing.Events
{

    public abstract class Event
    {
        protected Event(Guid id, DateTime date)
        {
            Id = id;
            Date = date;
        }

        public Guid Id { get; }

        public DateTime Date { get; }

        internal bool IsBefore(DateTime date)
        {
            return Date <= date;
        }
    }

}