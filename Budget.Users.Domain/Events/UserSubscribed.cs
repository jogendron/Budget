using System;

using Budget.EventSourcing.Events;

namespace Budget.Users.Domain.Events
{
    public class UserSubscribed : Event
    {
        public UserSubscribed(
            string userName,
            string firstName,
            string lastName,
            string email
        ) : base(Guid.NewGuid(), DateTime.Now)
        {
            UserId = Guid.NewGuid();
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public Guid UserId { get; }

        public string UserName { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}