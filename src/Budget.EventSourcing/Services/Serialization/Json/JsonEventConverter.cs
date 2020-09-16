using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Services.Serialization.Json
{
    public class JsonEventConverter : JsonConverter<Event>
    {
        private const string TypeNameProperty = "TypeName";
        private const string TypeValueProperty = "TypeValue";

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Event).IsAssignableFrom(typeToConvert);
        }

        public override Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Type concreteEventType = ReadEventType(ref reader);
            Event @event = ReadEvent(ref reader, concreteEventType);

            return @event;
        }

        public override void Write(Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var typeName = value.GetType().ToString();
            var assemblyName = value.GetType().Assembly.GetName().Name;
            writer.WriteString(TypeNameProperty, $"{typeName}, {assemblyName}");
            
            writer.WritePropertyName(TypeValueProperty);
            object concreteEvent = Convert.ChangeType(value, value.GetType());
            JsonSerializer.Serialize(writer, concreteEvent);

            writer.WriteEndObject();
        }

        private Type ReadEventType(ref Utf8JsonReader reader)
        {
            string concreteEventTypeName = string.Empty;

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            if (
                ! reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != TypeNameProperty
            )
            {
                throw new JsonException();
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.String)
                throw new JsonException();

            concreteEventTypeName = reader.GetString();

            return Type.GetType(concreteEventTypeName, true);
        }

        private Event ReadEvent(ref Utf8JsonReader reader, Type concreteEventType)
        {
            Event @event = null;

            if (!reader.Read() || reader.GetString() != TypeValueProperty)
                throw new JsonException();
            
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();
            
            @event = JsonSerializer.Deserialize(ref reader, concreteEventType) as Event;

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException();

            return @event;
        }

    }
}