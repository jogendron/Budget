using Budget.Users.Domain.WriteModel.Repositories;

namespace Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories
{
    public class InMemoryWriteModelUnitOfWork : IWriteModelUnitOfWork
    {
        public InMemoryWriteModelUnitOfWork(IWriteModelUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public IWriteModelUserRepository UserRepository { get; }

        public void BeginTransaction()
        {
            //We voluntarily do nothing here to keep it simple
            //since the in memory providers are stubs
        }

        public void Commit()
        {
            //We voluntarily do nothing here to keep it simple
            //since the in memory providers are stubs
        }

        public void Rollback()
        {
            //We voluntarily do nothing here to keep it simple
            //since the in memory providers are stubs
        }
    }
}