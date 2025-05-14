using Masasamjant.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;

namespace Masasamjant.Repositories.EntityFramework
{
    /// <summary>
    /// Represents entries of <typeparamref name="T"/> in repository that use Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">The type of the entry item.</typeparam>
    public sealed class EntityRepositoryEntries<T> : IRepositoryEntries<T>, IQueryable<T>, IQueryable where T : class
    {
        private readonly DbSet<T> set;

        /// <summary>
        /// Initializes new instance of the <see cref="EntityRepositoryEntries{T}"/> class.
        /// </summary>
        /// <param name="set">The <see cref="DbSet{T}"/>.</param>
        public EntityRepositoryEntries(DbSet<T> set)
        {
            this.set = set;
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add to the repository.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the added entity.</returns>
        public IRepositoryEntry<T> Add(T entity)
        {
            var entry = set.Add(entity);
            return new RepositoryEntry<T>(entry.Entity);
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add to the repository. Cannot be null.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the added entity.</returns>
        public async Task<IRepositoryEntry<T>> AddAsync(T entity, CancellationToken cancellationToken)
        {
            var entry = await set.AddAsync(entity, cancellationToken);
            return new RepositoryEntry<T>(entry.Entity);
        }

        /// <summary>
        /// Gets enumerator to iterate over the entries.
        /// </summary>
        /// <returns>A enumerator to iterate entries.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in set)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Remove the specified entity from the repository.    
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the removed entity.</returns>
        public IRepositoryEntry<T> Remove(T entity)
        {
            var entry = set.Remove(entity);
            return new RepositoryEntry<T>(entry.Entity);
        }

        /// <summary>
        /// Update the specified entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the updated entity.</returns>
        public IRepositoryEntry<T> Update(T entity)
        {
            var entry = set.Update(entity);
            return new RepositoryEntry<T>(entry.Entity);
        }

        /// <summary>
        /// Gets enumerator to iterate over the entries.
        /// </summary>
        /// <returns>A enumerator to iterate entries.</returns>
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return set.GetAsyncEnumerator(cancellationToken);
        }

        Type IQueryable.ElementType => ((IQueryable)set).ElementType;

        Expression IQueryable.Expression => ((IQueryable)set).Expression;

        IQueryProvider IQueryable.Provider => ((IQueryable)set).Provider;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
