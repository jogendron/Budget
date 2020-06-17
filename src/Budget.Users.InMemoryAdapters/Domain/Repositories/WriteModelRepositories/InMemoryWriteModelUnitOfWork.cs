using Budget.Users.Domain.Repositories.WriteModelRepositories;

namespace Budget.Users.InMemoryAdapters.Domain.Repositories.WriteModelRepositories
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