using System;

using Budget.EventSourcing.Events;

namespace Budget.Users.Domain.Events
{
    public class UserSubscribed : Event
    {
        //Constructor for serialization
        public UserSubscribed()
        {
            
        }

        public UserSubscribed(
            Guid aggregateId,
            string userName,
            string firstName,
            string lastName,
            string email
        ) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public string UserName { get; set; } //Setter for serialization

        public string FirstName { get; set; } //Setter for serialization

        public string LastName { get; set; } //Setter for serialization

        public string Email { get; set; } //Setter for serialization
    }
}