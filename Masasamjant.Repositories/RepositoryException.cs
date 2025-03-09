namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents exception thrown when repository operation fails.
    /// </summary>
    public class RepositoryException : Exception
    {
        /// <summary>
        /// Initializes new default instance of the <see cref="RepositoryException"/> class.
        /// </summary>
        public RepositoryException()
            : this(null)
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="RepositoryException"/> class
        /// </summary>
        /// <param name="queryText">The query text or text explaining query.</param>
        public RepositoryException(string? queryText)
            : this("The unexpected exception during repository operation.", queryText)
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="RepositoryException"/> class
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="queryText">The query text or text explaining query.</param>
        public RepositoryException(string message, string? queryText)
            : this(message, queryText, null)
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="RepositoryException"/> class
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="queryText">The query text or text explaining query.</param>
        /// <param name="innerException">The inner exception.</param>
        public RepositoryException(string message, string? queryText, Exception? innerException)
            : base(message, innerException)
        {
            QueryText = queryText ?? string.Empty;
        }

        /// <summary>
        /// Gets the query text or text explaining query or empty string.
        /// </summary>
        public string QueryText { get; }
    }
}
