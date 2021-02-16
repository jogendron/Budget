using System;

namespace Budget.Users.Domain.ReadModel
{
    public class User
    {
        public User(Guid id, string userName, string firstName, string lastName, string email)
        {
            Id = id;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public Guid Id { get; }

        public string UserName { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}