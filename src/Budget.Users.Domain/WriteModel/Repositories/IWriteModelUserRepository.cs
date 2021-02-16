using System;
using System.Threading.Tasks;

namespace Budget.Users.Domain.WriteModel.Repositories
{
    public interface IWriteModelUserRepository
    {
        Task<User> Get(Guid id);

        Task Save(User user);
    }
}