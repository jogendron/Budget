namespace Budget.Users.Domain.ReadModel.Repositories
{
    public interface IReadModelUnitOfWork
    {
        IReadModelUserRepository UserRepository { get; }

        void BeginTransaction();

        void Commit();

        void Rollback();
    }
}