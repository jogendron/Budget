namespace Budget.Users.Domain.Repositories.WriteModelRepositories
{
    public interface IWriteModelUnitOfWork : IUnitOfWork
    {
        IWriteModelUserRepository UserRepository { get; }
    }
}