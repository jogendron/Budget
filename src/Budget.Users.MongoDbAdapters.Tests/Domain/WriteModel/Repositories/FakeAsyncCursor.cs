using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Budget.Users.MongoDbAdapters.Tests.Domain.WriteModel.Repositories
{
    internal class FakeAsyncCursor<T> : IAsyncCursor<T> where T : class
    {
        private List<T> collection;
        private int position;
        
        public FakeAsyncCursor() : this(new List<T>())
        {
        }

        public FakeAsyncCursor(List<T> collection)
        {
            this.collection = collection;
            position = 0;
        }

        public IEnumerable<T> Current => collection.AsEnumerable();

        public void Dispose()
        {
        }

        public bool MoveNext(CancellationToken cancellationToken = default)
        {
            return position++ <= collection.Count;
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            return new Task<bool>(() => MoveNext(cancellationToken));
        }
    }
}