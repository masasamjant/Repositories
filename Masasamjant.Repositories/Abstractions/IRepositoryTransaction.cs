namespace Masasamjant.Repositories.Abstractions
{
    /// <summary>
    /// Represents a transaction for repository operations.
    /// </summary>
    public interface IRepositoryTransaction : IDisposable
    {
        /// <summary>
        /// Gets the identifier of the transaction.
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        /// Gets whether or not current transaction supports save points.
        /// </summary>
        bool IsSavePointsSupported { get; }

        /// <summary>
        /// Gets the timeout of rollback operation in seconds in case disposing and transaction 
        /// is still at uncommitted state.
        /// </summary>
        int RollbackTimeoutSeconds { get; }

        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        RepositoryTransactionState TransactionState { get; }

        /// <summary>
        /// Commit changes made in transaction.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">If commit is not possible.</exception>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rollback all changes made in transaction.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">If rollback is not possible.</exception>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rollback change made after save point.
        /// </summary>
        /// <param name="name">The save point name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="NotSupportedException">If transaction save points are not supported.</exception>
        /// <exception cref="InvalidOperationException">If rollback is not possible.</exception>
        Task RollbackAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create transaction save point.
        /// </summary>
        /// <param name="name">The save point name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="NotSupportedException">If transaction save points are not supported.</exception>
        /// <exception cref="InvalidOperationException">If save point creation is not possible.</exception>
        Task SavePointAsync(string name, CancellationToken cancellationToken = default);
    }
}
