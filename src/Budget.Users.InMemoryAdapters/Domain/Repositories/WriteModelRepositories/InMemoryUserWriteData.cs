using System;
using System.Collections.Generic;
using Budget.EventSourcing.Events;

namespace Budget.Users.InMemoryAdapters.Domain.Repositories.WriteModelRepositories
{
    public class InMemoryUserWriteData : Dictionary<Guid, IEnumerable<Event>>
    {
    }
}