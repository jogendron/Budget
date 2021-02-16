using Budget.Users.Domain.ReadModel.Repositories;

namespace Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories
{
    public class InMemoryReadModelUnitOfWork : IReadModelUnitOfWork
    {
        public InMemoryReadModelUnitOfWork(
            IReadModelUserRepository userRepository
        )
        {
            UserRepository = userRepository;
        }

        public IReadModelUserRepository UserRepository { get; }

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