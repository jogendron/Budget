using System;
using System.Threading.Tasks;

using Budget.Users.Domain.Model.WriteModel;

namespace Budget.Users.Domain.Repositories.WriteModelRepositories
{
    public interface IWriteModelUserRepository
    {
        Task<User> Get(Guid id);

        Task Save(User user);
    }
}