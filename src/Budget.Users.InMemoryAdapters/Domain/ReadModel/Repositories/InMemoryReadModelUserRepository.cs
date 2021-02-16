using System.Threading.Tasks;

using Budget.Users.Domain.ReadModel;
using Budget.Users.Domain.ReadModel.Repositories;

namespace Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories
{
    public class InMemoryReadModelUserRepository : IReadModelUserRepository
    {
        private InMemoryUserReadData Data { get; set;} 

        public InMemoryReadModelUserRepository(InMemoryUserReadData data)
        {
            Data = data;
        }

        public async Task<User> GetUser(string userName)
        {
            User user = null;

            if (Data.ContainsKey(userName))
                user = Data[userName];

            await Task.CompletedTask;

            return user;
        }

        public async Task Save(User user)
        {
            await Task.CompletedTask;

            if (Data.ContainsKey(user.UserName))
            {
                Data[user.UserName] = user;
            }
            else
            {
                Data.Add(user.UserName, user);
            }
        }

        public async Task<bool> Exists(string userName)
        {
            await Task.CompletedTask;
            return Data.ContainsKey(userName);
        }
    }
}