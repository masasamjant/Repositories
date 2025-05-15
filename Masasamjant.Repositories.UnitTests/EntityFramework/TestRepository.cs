using Masasamjant.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Masasamjant.Repositories.EntityFramework
{
    public class TestRepository : EntityRepository
    {
        private readonly Dictionary<Type, object> entries = new Dictionary<Type, object>();
        private readonly bool useMemoryEntries;
        private readonly Action? saveChangesCallback;

        public TestRepository(string connectionString, bool useMemoryEntries, Action? saveChangesCallback)
            : base(new ConnectionStringProvider(connectionString))
        {
            this.useMemoryEntries = useMemoryEntries;
            this.saveChangesCallback = saveChangesCallback;
        }

        protected override Task<int> DoSaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            saveChangesCallback?.Invoke();
            return Task.FromResult(1);
        }

        protected override IRepositoryEntries<T> GetEntries<T>()
        {
            if (useMemoryEntries)
            {
                var type = typeof(T);

                if (!entries.TryGetValue(type, out var entry))
                {
                    entry = new MemoryRepositoryEntries<T>();
                    entries.Add(type, entry);
                }

                return (IRepositoryEntries<T>)entry;
            }
            else
                return base.GetEntries<T>();
        }
    }
}
