using System;
using System.Threading;
using System.Threading.Tasks;

using Budget.Cqrs.Commands.EventProcessors;
using Budget.EventSourcing.Events;
using Budget.Users.InMemoryAdapters.Domain.Events;

using MediatR;

namespace Budget.Users.InMemoryAdapters.HostedServices
{
    public class EventProcessingLoop
    {
        public EventProcessingLoop(
            CancellationToken stoppingToken, 
            IMediator mediator, 
            InMemoryEventStream eventStream
        )
        {
            StoppingToken = stoppingToken;
            Mediator = mediator;
            EventStream = eventStream;   
        }

        private CancellationToken StoppingToken { get; }

        private IMediator Mediator { get; }

        private InMemoryEventStream EventStream { get; }

        public async Task Run()
        {
            Event @event = null;

            while (! StoppingToken.IsCancellationRequested)
            {
                if (EventStream.TryDequeue(out @event))
                {
                    Type commandType = typeof(ProcessEventCommand<>);
                    Type genericType = commandType.MakeGenericType(new Type[] { @event.GetType() });
                    object request = Activator.CreateInstance(genericType, new [] { @event });

                    try
                    {
                        await Mediator.Send(request);   
                    }
                    catch {}
                }
                else 
                {
                    await Task.Delay(100);
                }
            }
        }
    }
}