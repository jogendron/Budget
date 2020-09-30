using System.Text.Json;

using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Services.Serialization.Json
{
    public class JsonEventSerializer : IEventSerializer
    {
        public JsonEventSerializer()
        {
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new JsonEventConverter());
        }

        private JsonSerializerOptions Options { get; }

        public Event Deserialize(string content)
        {
            return JsonSerializer.Deserialize<Event>(content, Options);
        }

        public string Serialize<T>(T @event) where T : Event
        {
            return JsonSerializer.Serialize(@event, Options);
        }
    }

}