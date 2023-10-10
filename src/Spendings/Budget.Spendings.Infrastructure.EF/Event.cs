using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Budget.Spendings.Infrastructure.EF;

public class Event
{
    public Event()
    {
        Id = Guid.Empty;
        Type = string.Empty;
        Content = string.Empty;
    }

    public Event(EventSourcing.Events.Event @event)
    {
        Id = @event.EventId;
        
        var typeName = @event.GetType().ToString();
        var assemblyName = @event.GetType().Assembly.GetName().Name;
        Type = $"{typeName}, {assemblyName}";

        object concreteEvent = Convert.ChangeType(@event, @event.GetType());

        Content = JsonSerializer.Serialize(concreteEvent);
    }

    public Guid Id { get; set; }

    [MaxLength(500)]
    public string Type { get; set; }

    public string Content { get; set; }

    public EventSourcing.Events.Event ToDomainEvent()
    {
        EventSourcing.Events.Event @event;

        using (var ms = new MemoryStream())
        {
            using (var sw = new StreamWriter(ms))
            {
                sw.Write(Content);
                sw.Flush();
                ms.Position = 0;

                var eventType = System.Type.GetType(Type);

                if (eventType == null)
                    throw new Exception($"Could not cast event to unknown type {Type}");

                var eventObj = JsonSerializer.Deserialize(ms, eventType) as Budget.EventSourcing.Events.Event;  

                if (eventObj == null)
                    throw new Exception($"Failed to deserialize {Type} event.");

                @event = eventObj;
            }
        }    
            
        return @event;
    }
}