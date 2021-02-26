using System;

namespace Budget.Users.Domain.ReadModel
{
    public class User : IEquatable<User>
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

        public bool Equals(User other)
        {
            return
                this.Id == other.Id
                && this.UserName == other.UserName
                && this.FirstName == other.FirstName
                && this.LastName == other.LastName
                && this.Email == other.LastName;
        }
    }
}