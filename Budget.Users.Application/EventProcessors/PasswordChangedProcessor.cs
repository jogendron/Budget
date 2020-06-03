using System.Threading;
using System.Threading.Tasks;
using Budget.Cqrs.Commands.EventProcessors;
using Budget.Users.Domain.Events;

namespace Budget.Users.Application.EventProcessors
{
    public class PasswordChangedProcessor : EventProcessor<PasswordChanged>
    {
        public PasswordChangedProcessor()
        {
        }

        protected override async Task Handle(ProcessEventCommand<PasswordChanged> request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}