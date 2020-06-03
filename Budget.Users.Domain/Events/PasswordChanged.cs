using System;

using Budget.EventSourcing.Events;

namespace Budget.Users.Domain.Events
{
    public class PasswordChanged : Event
    {
        public PasswordChanged(string encryptedPassword) : base(Guid.NewGuid(), DateTime.Now)
        {
            EncryptedPassword = encryptedPassword;
        }

        public string EncryptedPassword { get; }
    }
}