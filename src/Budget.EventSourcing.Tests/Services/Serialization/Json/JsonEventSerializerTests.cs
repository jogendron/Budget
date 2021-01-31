using System;
using Xunit;
using FluentAssertions;

using Budget.EventSourcing.Events;
using Budget.EventSourcing.Services.Serialization.Json;
using Budget.EventSourcing.Tests.Events;

namespace Budget.EventSourcing.Tests.Services.Serialization.Json
{
    public class JsonEventSerializerTests
    {       
        FakeEvent @event = null;
        
        JsonEventSerializer serializer;

        public JsonEventSerializerTests()
        {
            @event = new FakeEvent(Guid.NewGuid(), "A knight says Ni.");

            serializer = new JsonEventSerializer();
        }

        [Fact]
        public void Write_WrapsValue_AddsTypeNameToWrapper()
        {
            //Arrange
            
            //Act
            string json = serializer.Serialize(@event);

            //Assert
            json.Should().Contain("TypeName");
            json.Should().Contain("TypeValue");
            json.Should().Contain(@event.GetType().ToString());
            json.Should().Contain(@event.AggregateId.ToString());
            json.Should().Contain(@event.Id.ToString());
            json.Should().Contain(@event.Date.ToString("d"));
            json.Should().Contain(@event.Message);
        }

        [Fact]
        public void Read_RecoversConcreteTypeProperties()
        {
            //Arrange
            string json = serializer.Serialize(@event);

            //Act
            Event deserializedEvent = serializer.Deserialize(json);
            FakeEvent deserializedFakeEvent = (FakeEvent) deserializedEvent;

            //Assert
            deserializedFakeEvent.AggregateId.Should().Be(@event.AggregateId);
            deserializedFakeEvent.Id.Should().Be(@event.Id);
            deserializedFakeEvent.Date.Should().Be(@event.Date);
            deserializedFakeEvent.Message.Should().Be(@event.Message);
        }

    }
}