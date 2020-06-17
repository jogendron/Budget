namespace Budget.Cqrs.Commands.EventProcessors
{
    public abstract class EventProcessor<TEvent> : CommandHandler<ProcessEventCommand<TEvent>> where TEvent : class
    {
    }
}