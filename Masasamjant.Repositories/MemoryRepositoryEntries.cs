using Masasamjant.Repositories.Abstractions;
using System.Collections;
using System.Linq.Expressions;

namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents entries of <typeparamref name="T"/> in repository stored in memory.
    /// </summary>
    /// <typeparam name="T">The type of the entry item.</typeparam>
    public sealed class MemoryRepositoryEntries<T> : IRepositoryEntries<T>, IQueryable<T>, IQueryable where T : class
    {
        private readonly List<T> entries;

        /// <summary>
        /// Initializes new empty instance of the <see cref="MemoryRepositoryEntries{T}"/> class.   
        /// </summary>
        public MemoryRepositoryEntries()
        {
            entries = new List<T>();
        }

        /// <summary>
        /// Initializes new instance of the <see cref="MemoryRepositoryEntries{T}"/> class.
        /// </summary>
        /// <param name="entries">The initial entries.</param>
        public MemoryRepositoryEntries(IEnumerable<T> entries)
            : this()
        {
            this.entries.AddRange(entries);
        }

        /// <summary>
        /// Gets enumerator to iterate over the entries.
        /// </summary>
        /// <returns>A enumerator to iterate entries.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var entry in entries)
            {
                yield return entry;
            }
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add to the repository.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the added entity.</returns>
        public IRepositoryEntry<T> Add(T entity)
        {
            var current = entries.FirstOrDefault(e => Equals(e, entity));

            if (current != null)
                return new RepositoryEntry<T>(current);

            entries.Add(entity);
            return new RepositoryEntry<T>(entity);
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add to the repository. Cannot be null.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the added entity.</returns>
        public Task<IRepositoryEntry<T>> AddAsync(T entity, CancellationToken cancellationToken)
        {
            var entry = Add(entity);
            return Task.FromResult(entry);
        }

        /// <summary>
        /// Remove the specified entity from the repository.    
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the removed entity.</returns>
        public IRepositoryEntry<T> Remove(T entity)
        {
            if (entries.Remove(entity))
            {
                return new RepositoryEntry<T>(entity);
            }
            else
                throw new InvalidOperationException("The entity is not part of the current set.");
        }

        /// <summary>
        /// Update the specified entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the updated entity.</returns>
        public IRepositoryEntry<T> Update(T entity)
        {
            var current = entries.FirstOrDefault(e => Equals(e, entity));

            if (current != null)
            {
                entries.Remove(current);
                entries.Add(entity);
                return new RepositoryEntry<T>(entity);
            }
            else
                throw new InvalidOperationException("The entity is not part of the current set.");
        }

        private IQueryable Queryable
        {
            get { return entries.AsQueryable(); }
        }

        Type IQueryable.ElementType => Queryable.ElementType;

        Expression IQueryable.Expression => Queryable.Expression;

        IQueryProvider IQueryable.Provider => Queryable.Provider;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
