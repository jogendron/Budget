using System;

namespace Budget.Users.Application.Commands.Subscribe
{
    public class SubscribeResponse
    {
        public SubscribeResponse(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}