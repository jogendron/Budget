using System;
using System.Threading;
using System.Threading.Tasks;

using Budget.Cqrs.Commands;
using Budget.EventSourcing.Events;
using Budget.Users.Application.Exceptions;
using Budget.Users.Domain.Model.WriteModel;
using Budget.Users.Domain.Factories.WriteModelFactories;
using Budget.Users.Domain.Repositories.ReadModelRepositories;
using Budget.Users.Domain.Repositories.WriteModelRepositories;

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

                await WriteModel.UserRepository.Save(user);
                await EventPublisher.PublishNewEvents(user);
                
                WriteModel.Commit();
            }
            catch (Exception ex)
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