namespace Masasamjant.Repositories
{
    /// <summary>
    /// Defines possible states of repository transaction.
    /// </summary>
    public enum RepositoryTransactionState : int
    {
        /// <summary>
        /// Changes made in transaction are not committed or reverted.
        /// </summary>
        Uncommitted = 0,

        /// <summary>
        /// Changes made in transaction have been committed.
        /// </summary>
        Committed = 1,

        /// <summary>
        /// Changes made in transaction have been reverted.
        /// </summary>
        Reverted = 2
    }
}
