using Masasamjant.Repositories.Abstractions;

namespace Masasamjant.Repositories.EntityFramework
{
    public class TestRepository : EntityRepository
    {
        private readonly Dictionary<Type, object> entries = new Dictionary<Type, object>();

        public TestRepository()
            : base(new ConnectionStringProvider("Repository"))
        { }

        protected override IRepositoryEntries<T> GetEntries<T>()
        {
            var type = typeof(T);

            if (!entries.TryGetValue(type, out var entry))
            {
                entry = new MemoryRepositoryEntries<T>();
                entries.Add(type, entry);
            }

            return (IRepositoryEntries<T>)entry;
        }
    }
}
