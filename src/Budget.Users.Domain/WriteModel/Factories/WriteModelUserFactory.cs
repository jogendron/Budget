using System;
using System.Collections.Generic;

using Budget.EventSourcing.Events;
using Budget.Users.Domain.WriteModel.Services;

namespace Budget.Users.Domain.WriteModel.Factories
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