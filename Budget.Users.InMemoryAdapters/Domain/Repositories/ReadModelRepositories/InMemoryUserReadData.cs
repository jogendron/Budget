using System.Collections.Generic;

using Budget.Users.Domain.Model.ReadModel;

namespace Budget.Users.InMemoryAdapters.Domain.Repositories.ReadModelRepositories
{
    public class InMemoryUserReadData : Dictionary<string, User>
    {
    }
}