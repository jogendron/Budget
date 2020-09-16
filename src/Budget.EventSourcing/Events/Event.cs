using System;

namespace Budget.EventSourcing.Events
{

    public abstract class Event
    {
        //Constructor for serialization
        protected Event()
        {

        }

        protected Event(Guid id, DateTime date)
        {
            Id = id;
            Date = date;
        }

        public Guid Id { get; set;} //Public setter because serialization requires it

        public DateTime Date { get; set;} //Public setter because serialization requires it

        internal bool IsBefore(DateTime date)
        {
            return Date <= date;
        }
    }

}