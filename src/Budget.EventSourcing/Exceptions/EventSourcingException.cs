using System;

namespace Budget.EventSourcing.Exceptions;

public abstract class EventSourcingException : Exception
{
    public EventSourcingException() : base()
    {
    }

    public EventSourcingException(string message) : base(message)
    {
    }

    public EventSourcingException(string message, Exception inner) : base(message, inner)
    {
    }
}