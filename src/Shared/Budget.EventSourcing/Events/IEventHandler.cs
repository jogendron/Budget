namespace Budget.EventSourcing.Events;

public interface IEventHandler<E> where E : Event
{
    void Handle(E @event);
}