using Microsoft.EntityFrameworkCore;

namespace Masasamjant.Repositories
{
    /// <summary>
    /// Provides helper methods related to repositories.
    /// </summary>
    public static class RepositoryHelper
    {
        /// <summary>
        /// Creates <see cref="RepositoryException"/> from specified details.
        /// </summary>
        /// <param name="operation">The name of repository operation.</param>
        /// <param name="queryText">The query text or <c>null</c>.</param>
        /// <param name="innerException">The occurred inner exception or <c>null</c>.</param>
        /// <returns>A <see cref="RepositoryException"/>.</returns>
        public static RepositoryException HandleRepositoryException(string operation, string? queryText, Exception? innerException)
            => new RepositoryException($"The {operation} operation failed.", queryText, innerException);

        /// <summary>
        /// Creates <see cref="RepositoryException"/> from specified details.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="operation">The name of repository operation.</param>
        /// <param name="queryText">The query text or <c>null</c>.</param>
        /// <param name="innerException">The occurred inner exception or <c>null</c>.</param>
        /// <returns>A <see cref="RepositoryException"/>.</returns>
        public static RepositoryException HandleRepositoryException(object instance, string operation, string? queryText, Exception? innerException)
            => new RepositoryException($"The {operation} operation of {instance.GetType()} failed.", queryText, innerException);

        /// <summary>
        /// Converts <see cref="DbUpdateConcurrencyException"/> to <see cref="ConcurrentUpdateException"/>.
        /// </summary>
        /// <param name="exception">The <see cref="DbUpdateConcurrencyException"/>.</param>
        /// <returns>A <see cref="ConcurrentUpdateException"/>.</returns>
        public static async Task<ConcurrentUpdateException> HandleConcurrentUpdateExceptionAsync(DbUpdateConcurrencyException exception)
        {
            var items = new List<ConcurrentUpdateItem>(exception.Entries.Count);

            if (exception.Entries.Count == 0)
                return new ConcurrentUpdateException(items);

            foreach (var entry in exception.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync();
                object currentInstance = entry.Entity;
                object? repositoryInstance = databaseValues?.ToObject();
                items.Add(new ConcurrentUpdateItem(currentInstance, repositoryInstance));
            }

            return new ConcurrentUpdateException(items);
        }
    }
}
