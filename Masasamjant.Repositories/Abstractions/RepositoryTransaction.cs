namespace Masasamjant.Repositories.Abstractions
{
    /// <summary>
    /// Represents abstract transaction for repository operations.
    /// </summary>
    public abstract class RepositoryTransaction : IRepositoryTransaction, IDisposable
    {
        /// <summary>
        /// Default value of <see cref="RollbackTimeoutSeconds"/>.
        /// </summary>
        public const int DefaultRollbackTimeoutSeconds = 30;

        /// <summary>
        /// Initializes new instance of the <see cref="RepositoryTransaction"/> class.
        /// </summary>
        /// <param name="rollbackTimeoutSeconds">The timeout of rollback operation in seconds in case disposing and transaction is still at uncommitted state.</param>
        /// <exception cref="ArgumentOutOfRangeException">If value of <paramref name="rollbackTimeoutSeconds"/> is less than 1.</exception>
        protected RepositoryTransaction(int rollbackTimeoutSeconds = DefaultRollbackTimeoutSeconds)
        {
            if (rollbackTimeoutSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(rollbackTimeoutSeconds), rollbackTimeoutSeconds, "The value must be greater than 0.");

            RollbackTimeoutSeconds = rollbackTimeoutSeconds;
        }

        /// <summary>
        /// Finalizes current instance.
        /// </summary>
        ~RepositoryTransaction()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        public RepositoryTransactionState TransactionState { get; protected set; }

        /// <summary>
        /// Gets whether or not transaction is disposed.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Gets the identifier of the transaction.
        /// </summary>
        public abstract Guid Identifier { get; }

        /// <summary>
        /// Gets whether or not current transaction supports save points.
        /// </summary>
        public abstract bool IsSavePointsSupported { get; }

        /// <summary>
        /// Gets the timeout of rollback operation in seconds in case disposing and transaction 
        /// is still at uncommitted state.
        /// </summary>
        public int RollbackTimeoutSeconds { get; }

        /// <summary>
        /// Commit changes made in transaction.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">If commit is not possible.</exception>
        public abstract Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rollback all changes made in transaction.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">If rollback is not possible.</exception>
        public abstract Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rollback change made after save point.
        /// </summary>
        /// <param name="name">The save point name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="NotSupportedException">If transaction save points are not supported.</exception>
        /// <exception cref="InvalidOperationException">If rollback is not possible.</exception>
        public abstract Task RollbackAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create transaction save point.
        /// </summary>
        /// <param name="name">The save point name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="NotSupportedException">If transaction save points are not supported.</exception>
        /// <exception cref="InvalidOperationException">If save point creation is not possible.</exception>
        public abstract Task SavePointAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Disposes current instance and rollback transaction if still uncommitted.
        /// </summary>
        public void Dispose()
        { 
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes current instance and rollback transaction if still uncommitted.
        /// </summary>
        /// <param name="disposing"><c>true</c> if disposing; <c>false</c> otherwise.</param>
        protected abstract void Dispose(bool disposing);
    }
}
