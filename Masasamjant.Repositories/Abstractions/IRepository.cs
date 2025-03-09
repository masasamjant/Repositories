namespace Masasamjant.Repositories.Abstractions
{
    /// <summary>
    /// Represents repository to save data of objects.
    /// </summary>
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Add specified <typeparamref name="T"/> instance to repository.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The <typeparamref name="T"/> instance to add.</param>
        /// <param name="saveChanges"><c>true</c> to save changes immediately; <c>false</c> if <see cref="SaveChangesAsync"/> is invoked afterwards.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A added <typeparamref name="T"/> instance.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="ConcurrentUpdateException">If concurrent update error occurs.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        Task<T> AddAsync<T>(T instance, bool saveChanges = false, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Check if repository contains any instance of <typeparamref name="T"/> that match specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="specification">The criteria specification to match.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><c>true</c> if repository contains any instance of <typeparamref name="T"/> that match criteria of <paramref name="specification"/>; <c>false</c> otherwise.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        Task<bool> ExistsAsync<T>(IQuerySpecification<T> specification, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Gets all instance of <typeparamref name="T"/> in repository. This method can be used in case the amount of instance of <typeparamref name="T"/> is limited 
        /// and it is known that all those are needed.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of <typeparamref name="T"/> instances in repository.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        Task<List<T>> GetAsync<T>(CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Gets all instance of <typeparamref name="T"/> in repository that match specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="specification">The criteria specification to match.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of <typeparamref name="T"/> instances in repository that match criteria of <paramref name="specification"/>.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        Task<List<T>> GetAsync<T>(IQuerySpecification<T> specification, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Query instances of <typeparamref name="T"/> in repository.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>A query of <typeparamref name="T"/> instance in repository.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        IQueryable<T> Query<T>() where T : class;

        /// <summary>
        /// Query instances of <typeparamref name="T"/> in repository that match specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="specification">The criteria specification to match.</param>
        /// <returns>A query of <typeparamref name="T"/> instance in repository that match criteria of <paramref name="specification"/>.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        IQueryable<T> Query<T>(IQuerySpecification<T> specification) where T : class;

        /// <summary>
        /// Remove, delete permanently, instance of <typeparamref name="T"/> from repository.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The instance to remove.</param>
        /// <param name="saveChanges"><c>true</c> to save changes immediately; <c>false</c> if <see cref="SaveChangesAsync"/> is invoked afterwards.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A removed instance.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="ConcurrentUpdateException">If concurrent update error occurs.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        Task<T> RemoveAsync<T>(T instance, bool saveChanges = false, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Save changes made to instance stored in repository.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A count of changed instances.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="ConcurrentUpdateException">If concurrent update error occurs.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Update specified instance of <typeparamref name="T"/> in repository.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The instance to update.</param>
        /// <param name="saveChanges"><c>true</c> to save changes immediately; <c>false</c> if <see cref="SaveChangesAsync"/> is invoked afterwards.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A updated instance.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="ConcurrentUpdateException">If concurrent update error occurs.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        Task<T> UpdateAsync<T>(T instance, bool saveChanges = false, CancellationToken cancellationToken = default) where T : class;
    }
}
