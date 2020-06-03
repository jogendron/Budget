using System.Threading.Tasks;
using Budget.Users.Domain.Model.ReadModel;

namespace Budget.Users.Domain.Repositories.ReadModelRepositories
{
    public interface IReadModelUserRepository
    {
        Task<User> GetUser(string userName);

        Task<bool> Exists(string userName);

        Task Save(User user);
    }
}