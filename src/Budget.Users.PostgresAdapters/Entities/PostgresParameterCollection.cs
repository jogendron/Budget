using System.Collections;
using System.Collections.Generic;
using Npgsql;

namespace Budget.Users.PostgresAdapters.Entities
{
    public class PostgresParameterCollection : IPostgresParameterCollection
    {
        public PostgresParameterCollection(NpgsqlParameterCollection collection)
        {
            Collection = collection;
            CollectionWrapper = new List<IPostgresParameter>();
        }

        public NpgsqlParameterCollection Collection { get; }

        private List<IPostgresParameter> CollectionWrapper { get; }

        public void Add(IPostgresParameter parameter)
        {
            CollectionWrapper.Add(parameter);
            Collection.Add( ((PostgresParameter)parameter).Parameter );
        }

        public IEnumerator<IPostgresParameter> GetEnumerator()
        {
            return CollectionWrapper.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}