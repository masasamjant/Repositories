namespace Masasamjant.Repositories.Abstractions
{
    /// <summary>
    /// Represents entries of <typeparamref name="T"/> in repository.
    /// </summary>
    /// <typeparam name="T">The type of the entry item.</typeparam>
    public interface IRepositoryEntries<T> : IQueryable<T>, IQueryable, IAsyncEnumerable<T> where T : class
    {
        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add to the repository.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the added entity.</returns>
        IRepositoryEntry<T> Add(T entity);

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add to the repository. Cannot be null.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the added entity.</returns>
        Task<IRepositoryEntry<T>> AddAsync(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Remove the specified entity from the repository.    
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the removed entity.</returns>
        IRepositoryEntry<T> Remove(T entity);

        /// <summary>
        /// Update the specified entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>An <see cref="IRepositoryEntry{T}"/> representing the updated entity.</returns>
        IRepositoryEntry<T> Update(T entity);
    }
}
