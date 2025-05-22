namespace Masasamjant.Repositories.Abstractions
{
    /// <summary>
    /// Represents a scope of repository transaction. This ensures that when the scope is disposed, 
    /// then associated transaction is rollbacked if it has not been committed.
    /// </summary>
    public interface IRepositoryTransactionScope : IDisposable
    {
        /// <summary>
        /// Gets the scoped repository transaction.
        /// </summary>
        IRepositoryTransaction Transaction { get; }
    }
}
