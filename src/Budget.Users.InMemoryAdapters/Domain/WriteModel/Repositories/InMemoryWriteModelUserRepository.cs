using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Budget.EventSourcing.Events;
using Budget.Users.Domain.WriteModel;
using Budget.Users.Domain.WriteModel.Factories;
using Budget.Users.Domain.WriteModel.Repositories;

namespace Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories
{
    public class InMemoryWriteModelUserRepository : IWriteModelUserRepository
    {
        public InMemoryWriteModelUserRepository(WriteModelUserFactory userFactory, InMemoryUserWriteData data)
        {
            UserFactory = userFactory;
            Data = data;
        }

        private WriteModelUserFactory UserFactory { get; }

        private Dictionary<Guid, IEnumerable<Event>> Data { get; }      

        public async Task<User> Get(Guid id)
        {
            User user = null;
            
            if (Data.ContainsKey(id))
            {
                IEnumerable<Event> events = Data[id];
                user = UserFactory.Load(id, events);
            }          

            await Task.CompletedTask;

            return user;
        }

        public async Task Save(User user)
        {
            if (Data.ContainsKey(user.Id))
                Data[user.Id] = user.Changes;
            else
                Data.Add(user.Id, user.Changes);

            await Task.CompletedTask;
        }
    }
}
