using System;
using System.Collections.Generic;

using Budget.EventSourcing.Events;
using Budget.Users.Domain.Model.WriteModel;
using Budget.Users.Domain.Services;

namespace Budget.Users.Domain.Factories.WriteModelFactories
{
    public class WriteModelUserFactory 
    {
        public WriteModelUserFactory(ICryptService cryptService)
        {
            CryptService = cryptService;
        }

        private ICryptService CryptService { get; }

        public User Create(
            string userName, 
            string firstName,
            string lastName,
            string email,
            string password
        )
        {
            var encryptedPassword = CryptService.Crypt(password);

            return new User(
                userName,
                firstName,
                lastName,
                email,
                encryptedPassword
            );
        }

        public User Load(
            Guid id,
            IEnumerable<Event> changes
        )
        {
            User user = new User(id, changes);
            user.ApplyChangeHistory();

            return user;
        }
    }
}