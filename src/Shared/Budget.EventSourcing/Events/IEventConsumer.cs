using System.Threading.Tasks;

namespace Budget.EventSourcing.Events;

public interface IEventConsumer<T> where T : Event
{
    Task<T> Consume();

    Task CommitOffset();
}