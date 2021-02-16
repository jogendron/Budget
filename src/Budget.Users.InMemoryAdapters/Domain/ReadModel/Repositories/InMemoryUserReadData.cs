using System.Collections.Generic;

using Budget.Users.Domain.ReadModel;

namespace Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories
{
    public class InMemoryUserReadData : Dictionary<string, User>
    {
    }
}