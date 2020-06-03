using System.Threading;
using System.Threading.Tasks;
using Budget.Cqrs.Commands.EventProcessors;
using Budget.Users.Domain.Events;
using Budget.Users.Domain.Model.ReadModel;
using Budget.Users.Domain.Repositories.ReadModelRepositories;

namespace Budget.Users.Application.EventProcessors
{
    public class UserSubscribedProcessor : EventProcessor<UserSubscribed>
    {
        public UserSubscribedProcessor(IReadModelUnitOfWork readModel)
        {
            ReadModel = readModel;
            
        }

        private IReadModelUnitOfWork ReadModel { get; }

        protected override async Task Handle(ProcessEventCommand<UserSubscribed> request, CancellationToken cancellationToken)
        {
            try
            {
                ReadModel.BeginTransaction();

                UserSubscribed subscription = request.Event;

                User user = new User(
                    subscription.Id,
                    subscription.UserName,
                    subscription.FirstName,
                    subscription.LastName,
                    subscription.Email
                );

                await ReadModel.UserRepository.Save(user);

                ReadModel.Commit();
            }
            catch
            {
                ReadModel.Rollback();
            }
        }
    }
}