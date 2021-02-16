using System;
using System.Collections.Generic;
using Budget.EventSourcing.Events;

namespace Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories
{
    public class InMemoryUserWriteData : Dictionary<Guid, IEnumerable<Event>>
    {
    }
}