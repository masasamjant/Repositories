using Masasamjant.Repositories.Abstractions;

namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents an entry of <typeparamref name="T"/> in repository.
    /// </summary>
    /// <typeparam name="T">The type of the entry item.</typeparam>
    public class RepositoryEntry<T> : IRepositoryEntry<T> where T : class
    {
        /// <summary>
        /// Initializes new instance of the <see cref="RepositoryEntry{T}"/> class.
        /// </summary>
        /// <param name="entity">The entry item.</param>
        public RepositoryEntry(T entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets the entry item.
        /// </summary>
        public T Entity { get; }
    }
}
