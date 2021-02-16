namespace Budget.Users.Domain.WriteModel.Repositories
{
    public interface IWriteModelUnitOfWork
    {
        IWriteModelUserRepository UserRepository { get; }

        void BeginTransaction();

        void Commit();

        void Rollback();
    }
}