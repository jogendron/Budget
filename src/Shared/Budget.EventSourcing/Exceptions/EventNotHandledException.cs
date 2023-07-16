using System;

namespace Budget.EventSourcing.Exceptions;

public class EventNotHandledException : EventSourcingException
{
    public EventNotHandledException() : base()
    {
    }

    public EventNotHandledException(string message) : base(message)
    {
    }

    public EventNotHandledException(string message, Exception inner) : base(message, inner)
    {
    }
}