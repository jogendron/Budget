using System;
using System.Collections.Generic;
using System.Net.Mail;

using Budget.EventSourcing.Entities;
using Budget.EventSourcing.Events;
using Budget.Users.Domain.Events;

namespace Budget.Users.Domain.Model.WriteModel
{
    public class User 
        : Aggregate,
        IEventHandler<UserSubscribed>,
        IEventHandler<PasswordChanged>   
    {
        private string userName = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string encryptedPassword = string.Empty;

        internal User(
            string userName, 
            string firstName, 
            string lastName, 
            string email, 
            string encryptedPassword
        ) : base()
        {
            var subscription = new UserSubscribed(userName, firstName, lastName, email);
            var passwordChange = new PasswordChanged(encryptedPassword);

            AddChange(subscription);
            AddChange(passwordChange);
        }

        internal User(Guid id, IEnumerable<Event> changes) : base(id, changes)
        {
        }

        public string UserName 
        { 
            get { return this.userName; }
            private set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("User name cannot be null or empty.");

                this.userName = value;
            } 
        }

        public string FirstName 
        { 
            get { return this.firstName; }
            private set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("First name cannot be null or empty.");

                this.firstName = value;
            } 
        }

        public string LastName 
        { 
            get { return this.lastName; }
            private set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Last name cannot be null or empty.");

                this.lastName = value;
            } 
        }

        public MailAddress Email { get; private set; }

        internal string EncryptedPassword 
        { 
            get { return this.encryptedPassword; }
            private set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Encrypted password cannot be null or empty.");

                this.encryptedPassword = value;
            } 
        }

        void IEventHandler<UserSubscribed>.Handle(UserSubscribed @event)
        {
            Id = @event.UserId;
            UserName = @event.UserName;
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            Email = new MailAddress(@event.Email);
        }

        void IEventHandler<PasswordChanged>.Handle(PasswordChanged @event)
        {
            EncryptedPassword = @event.EncryptedPassword;
        }

    }
}