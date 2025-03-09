namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents exception thrown when concurrent update occurs.
    /// </summary>
    public class ConcurrentUpdateException : Exception
    {
        /// <summary>
        /// Initializes new default instance of the <see cref="ConcurrentUpdateException"/> class.
        /// </summary>
        public ConcurrentUpdateException()
            : this(null)
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="ConcurrentUpdateException"/> class.
        /// </summary>
        /// <param name="items">The items related to concurrent update.</param>
        public ConcurrentUpdateException(IEnumerable<ConcurrentUpdateItem>? items)
            : this("The concurrent update error occurred.", items)
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="ConcurrentUpdateException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="items">The items related to concurrent update.</param>
        public ConcurrentUpdateException(string message, IEnumerable<ConcurrentUpdateItem>? items)
            : this(message, items, null) 
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="ConcurrentUpdateException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="items">The items related to concurrent update.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConcurrentUpdateException(string message, IEnumerable<ConcurrentUpdateItem>? items, Exception? innerException)
            : base(message, innerException) 
        {
            Items = items ?? [];
        }
        
        /// <summary>
        /// Gets the items related to concurrent update.
        /// </summary>
        public IEnumerable<ConcurrentUpdateItem> Items { get; }
    }
}
