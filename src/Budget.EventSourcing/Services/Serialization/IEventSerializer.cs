using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Services.Serialization
{
    public interface IEventSerializer
    {
        string Serialize<T>(T @event) where T: Event;

        Event Deserialize(string content);
    }
}