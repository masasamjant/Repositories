using Masasamjant.Repositories.Abstractions;

namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents connection string provider of specified value.
    /// </summary>
    public sealed class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes new instance of the <see cref="ConnectionStringProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to provide.</param>
        /// <exception cref="ArgumentException">If <paramref name="connectionString"/> is empty or only white-space.</exception>
        public ConnectionStringProvider(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("The value cannot be empty or only white-space.", nameof(connectionString));

            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>A connection string.</returns>
        public string GetConnectionString()
        {
            return connectionString;
        }
    }
}
