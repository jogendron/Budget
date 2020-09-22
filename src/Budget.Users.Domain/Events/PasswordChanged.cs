using System;

using Budget.EventSourcing.Events;

namespace Budget.Users.Domain.Events
{
    public class PasswordChanged : Event
    {
        public PasswordChanged(Guid aggregateId, string encryptedPassword) : base(aggregateId, Guid.NewGuid(), DateTime.Now)
        {
            EncryptedPassword = encryptedPassword;
        }

        public string EncryptedPassword { get; }
    }
}