using System;

using Budget.EventSourcing.Events;

namespace Budget.Users.Domain.Events
{
    public class UserSubscribed : Event
    {
        public UserSubscribed(
            Guid aggregateId,
            string userName,
            string firstName,
            string lastName,
            string email
        ) : base(aggregateId, Guid.NewGuid(), DateTime.Now)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public string UserName { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}