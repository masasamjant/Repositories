namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents items related to concurrent update error.
    /// </summary>
    public sealed class ConcurrentUpdateItem
    {
        /// <summary>
        /// Initializes new instance of the <see cref="ConcurrentUpdateItem"/> class.
        /// </summary>
        /// <param name="currentInstance">The current object instance.</param>
        /// <param name="repositoryInstance">The object instance with values from data source.</param>
        public ConcurrentUpdateItem(object currentInstance, object? repositoryInstance)
        { 
            CurrentInstance = currentInstance;
            RepositoryInstance = repositoryInstance;
        }

        /// <summary>
        /// Gets the current object instance.
        /// </summary>
        public object CurrentInstance { get; }

        /// <summary>
        /// Gets the object instance with values from data source.
        /// </summary>
        public object? RepositoryInstance { get; }
    }
}
