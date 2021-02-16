using System.Threading.Tasks;

namespace Budget.Users.Domain.ReadModel.Repositories
{
    public interface IReadModelUserRepository
    {
        Task<User> GetUser(string userName);

        Task<bool> Exists(string userName);

        Task Save(User user);
    }
}