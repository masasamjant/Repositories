using Masasamjant.Repositories.Abstractions;

namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents a scope of repository transaction. This ensures that when the scope is disposed, 
    /// then associated transaction is rollbacked if it has not been committed.
    /// </summary>
    public sealed class RepositoryTransactionScope : IRepositoryTransactionScope
    {
        private readonly object mutex;

        /// <summary>
        /// Initializes new instance of the <see cref="RepositoryTransactionScope"/> class with specified transaction.
        /// </summary>
        /// <param name="transaction">The repository transaction.</param>
        /// <exception cref="ArgumentException">If <paramref name="transaction"/> is in reverted state.</exception>
        public RepositoryTransactionScope(IRepositoryTransaction transaction)
        {
            if (transaction.TransactionState == RepositoryTransactionState.Reverted)
                throw new ArgumentException("The transaction is in reverted state.", nameof(transaction));

            Transaction = transaction;
            mutex = new object();
            IsDisposed = false;
        }

        /// <summary>
        /// Finalizes current instance.
        /// </summary>
        ~RepositoryTransactionScope()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the scoped repository transaction.
        /// </summary>
        public IRepositoryTransaction Transaction { get; }

        /// <summary>
        /// Gets whether or not instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes the current instance and rollbacks associated transaction, if it has not been committed.
        /// </summary>
        /// <exception cref="TimeoutException">If transaction rollback timeouts.</exception>
        /// <exception cref="InvalidOperationException">If transaction rollback fails.</exception>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            lock (mutex)
            {
                if (IsDisposed)
                    return;

                IsDisposed = true;

                if (Transaction.TransactionState == RepositoryTransactionState.Uncommitted)
                {
                    var task = Transaction.RollbackAsync();
                    if (task.Wait(TimeSpan.FromSeconds(Transaction.RollbackTimeoutSeconds)))
                        throw new TimeoutException($"Transaction rollback timeouted in {Transaction.RollbackTimeoutSeconds} seconds.");
                }

                if (disposing)
                    Transaction.Dispose();
            }
        }
    }
}
