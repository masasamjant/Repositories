using Masasamjant.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Masasamjant.Repositories.EntityFramework
{
    /// <summary>
    /// Represents transaction for repository operations of <see cref="EntityRepository"/>.
    /// </summary>
    public class EntityRepositoryTransaction : RepositoryTransaction
    {
        private readonly IDbContextTransaction transaction;

        /// <summary>
        /// Initializes new instance of the <see cref="EntityRepositoryTransaction"/> class.
        /// </summary>
        /// <param name="transaction">The entity framework DB context transaction.</param>
        internal protected EntityRepositoryTransaction(IDbContextTransaction transaction, int rollbackTimeoutSeconds = DefaultRollbackTimeoutSeconds)
            : base(rollbackTimeoutSeconds)
        {
            this.transaction = transaction;
            TransactionState = RepositoryTransactionState.Uncommitted;
        }

        /// <summary>
        /// Gets the identifier of the transaction.
        /// </summary>
        public override Guid Identifier => transaction.TransactionId;

        /// <summary>
        /// Gets whether or not current transaction supports save points.
        /// </summary>
        public override bool IsSavePointsSupported => transaction.SupportsSavepoints;

        /// <summary>
        /// Commit changes made in transaction.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">If commit is not possible.</exception>
        public override async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (TransactionState == RepositoryTransactionState.Committed)
                return;

            CheckDisposed();

            if (TransactionState == RepositoryTransactionState.Uncommitted)
            {
                await transaction.CommitAsync(cancellationToken);
                TransactionState = RepositoryTransactionState.Committed;
            }
            else
                throw new InvalidOperationException("The transaction has already been reverted and commit is not possible.");
        }

        /// <summary>
        /// Rollback all changes made in transaction.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">If rollback is not possible.</exception>
        public override async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (TransactionState == RepositoryTransactionState.Reverted)
                return;

            CheckDisposed();

            if (TransactionState == RepositoryTransactionState.Uncommitted)
            {
                await transaction.RollbackAsync(cancellationToken);
                TransactionState = RepositoryTransactionState.Reverted;
            }
            else
                throw new InvalidOperationException("The transaction has already been committed and rollback is not possible.");
        }

        /// <summary>
        /// Rollback change made after save point.
        /// </summary>
        /// <param name="name">The save point name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="NotSupportedException">If transaction save points are not supported.</exception>
        /// <exception cref="InvalidOperationException">If rollback is not possible.</exception>
        public override async Task RollbackAsync(string name, CancellationToken cancellationToken = default)
        {
            if (!IsSavePointsSupported)
                throw new NotSupportedException("The current transaction does not support save points.");

            CheckDisposed();

            if (TransactionState == RepositoryTransactionState.Uncommitted)
            {
                await transaction.RollbackToSavepointAsync(name, cancellationToken);
            }
            else
                throw new InvalidOperationException("The rollback to save point is only possible when transaction has not been committed or reverted.");
        }

        /// <summary>
        /// Create transaction save point.
        /// </summary>
        /// <param name="name">The save point name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="NotSupportedException">If transaction save points are not supported.</exception>
        /// <exception cref="InvalidOperationException">If save point creation is not possible.</exception>
        public override async Task SavePointAsync(string name, CancellationToken cancellationToken = default)
        {
            if (!IsSavePointsSupported)
                throw new NotSupportedException("The current transaction does not support save points.");

            CheckDisposed();

            if (TransactionState == RepositoryTransactionState.Uncommitted)
            {
                await transaction.CreateSavepointAsync(name, cancellationToken);
            }
            else
                throw new InvalidOperationException("The save point creation is only possible when transaction is in uncommitted state.");
        }

        /// <summary>
        /// Disposes current instance and rollback transaction if still uncommitted.
        /// </summary>
        /// <param name="disposing"><c>true</c> if disposing; <c>false</c> otherwise.</param>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            if (TransactionState == RepositoryTransactionState.Uncommitted)
            {
                var task = transaction.RollbackAsync();
                if (task.Wait(TimeSpan.FromSeconds(RollbackTimeoutSeconds)))
                    throw new TimeoutException($"Transaction rollback timeouted in {RollbackTimeoutSeconds} seconds.");
            }

            if (disposing)
                transaction.Dispose();
        }

        private void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
