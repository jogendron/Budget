using System.Threading;
using System.Threading.Tasks;

using Budget.Cqrs.Commands;
using Budget.EventSourcing.Events;
using Budget.Users.Application.Exceptions;
using Budget.Users.Domain.WriteModel;
using Budget.Users.Domain.WriteModel.Factories;
using Budget.Users.Domain.WriteModel.Repositories;
using Budget.Users.Domain.ReadModel.Repositories;

namespace Budget.Users.Application.Commands.Subscribe
{
    public class SubscribeHandler : CommandHandler<SubscribeCommand>
    {
        public SubscribeHandler(
            WriteModelUserFactory userFactory, 
            IReadModelUnitOfWork readModel,
            IWriteModelUnitOfWork writeModel,
            IEventPublisher eventPublisher
        )
        {
            UserFactory = userFactory;
            ReadModel = readModel;
            WriteModel = writeModel;
            EventPublisher = eventPublisher;
        }

        private WriteModelUserFactory UserFactory { get; }

        private IReadModelUnitOfWork ReadModel { get; }

        private IWriteModelUnitOfWork WriteModel { get; }

        private IEventPublisher EventPublisher { get; }

        protected override async Task Handle(SubscribeCommand command, CancellationToken token)
        {
            try
            {
                if (ExistsAlready(command.UserName))
                    throw new UserAlreadyExistsException();
                
                WriteModel.BeginTransaction();

                var user = CreateUser(command);

                await Task.WhenAll(
                    new Task[] {
                        WriteModel.UserRepository.Save(user),
                        EventPublisher.PublishNewEvents(user)
                    }
                );

                WriteModel.Commit();
            }
            catch
            {
                WriteModel.Rollback();
                throw;
            }
        }

        private bool ExistsAlready(string userName)
        {
            Task<bool> existenceCheck = ReadModel.UserRepository.Exists(userName);
            existenceCheck.Wait();

            return existenceCheck.Result;
        }

        private User CreateUser(SubscribeCommand command)
        {
            return UserFactory.Create(
                command.UserName,
                command.FirstName,
                command.LastName,
                command.Email,
                command.Password
            );
        }

    }
}