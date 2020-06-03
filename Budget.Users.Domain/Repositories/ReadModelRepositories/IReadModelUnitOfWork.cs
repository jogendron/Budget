namespace Budget.Users.Domain.Repositories.ReadModelRepositories
{
    public interface IReadModelUnitOfWork : IUnitOfWork
    {
        IReadModelUserRepository UserRepository { get; }
    }
}