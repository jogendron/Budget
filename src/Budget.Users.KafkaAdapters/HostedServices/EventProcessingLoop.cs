using System;
using System.Threading;
using System.Threading.Tasks;

using Budget.Cqrs.Commands.EventProcessors;
using Budget.EventSourcing.Events;
using Budget.EventSourcing.Services.Serialization;
using Budget.Users.KafkaAdapters.Entities;

using Confluent.Kafka;
using MediatR;

namespace Budget.Users.KafkaAdapters.HostedServices
{
    public class EventProcessingLoop
    {
        public EventProcessingLoop(
            CancellationToken stoppingToken, 
            IConsumer<Ignore, string> consumer,
            IEventSerializer eventSerializer,
            IMediator mediator
        )
        {
            StoppingToken = stoppingToken;
            Consumer = consumer;
            EventSerializer = eventSerializer;
            Mediator = mediator;
        }

        private CancellationToken StoppingToken { get; }

        private IConsumer<Ignore, string> Consumer { get; }

        private IEventSerializer EventSerializer { get; }

        private IMediator Mediator { get; }

        public async Task Run()
        {
            Consumer.Subscribe(KafkaTopics.UserEventSourcing);

            while (! StoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Ignore, string> result = await Task.Run(() => Consumer.Consume(StoppingToken));
                string value = result?.Message?.Value;

                if (! string.IsNullOrEmpty(value))
                {
                    Event @event = EventSerializer.Deserialize(value);

                    Type commandType = typeof(ProcessEventCommand<>);
                    Type genericType = commandType.MakeGenericType(new Type[] { @event.GetType() });
                    object request = Activator.CreateInstance(genericType, new [] { @event });

                    try
                    {
                        await Mediator.Send(request);   
                    }
                    catch {}
                }                
            }
        }
    }
}