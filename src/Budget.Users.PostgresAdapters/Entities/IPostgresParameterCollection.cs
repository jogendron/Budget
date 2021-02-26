using System.Collections.Generic;

namespace Budget.Users.PostgresAdapters.Entities
{
    public interface IPostgresParameterCollection : IEnumerable<IPostgresParameter>
    {
        void Add(IPostgresParameter parameter);
    }
} 