namespace Budget.Cqrs.Commands.EventProcessors
{
    public class ProcessEventCommand<TEvent> : ICommand where TEvent : class
    {
        public ProcessEventCommand(TEvent @event)
        {
            @Event = @event;
        }

        public TEvent @Event { get; }
    }
}