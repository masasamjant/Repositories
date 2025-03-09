using Masasamjant.Repositories.Abstractions;
using Masasamjant.Repositories.Configuration;
using Microsoft.Extensions.Configuration;

namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents provider of connection string that reads connection string value from configuration.
    /// </summary>
    public sealed class ConnectionStringProvider : IConnectionStringProvider
    {
        private IConfiguration configuration;
        private readonly IEnumerable<string> sectionKeys;
        private readonly string connectionStringKey;

        /// <summary>
        /// Initializes new instance of the <see cref="ConnectionStringProvider"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="connectionStringKey">The configuration key of connection string value.</param>
        /// <param name="sectionKeys">The names of configuration sections.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="connectionStringKey"/> is empty or contains only whitespace characters.</exception>
        public ConnectionStringProvider(IConfiguration configuration, string connectionStringKey, IEnumerable<string>? sectionKeys = null)
        {
            if (string.IsNullOrWhiteSpace(connectionStringKey))
                throw new ArgumentNullException(nameof(connectionStringKey), "The connection string key cannot be empty or only whitespace characters.");

            this.configuration = configuration;
            this.sectionKeys = sectionKeys ?? [];
            this.connectionStringKey = connectionStringKey;
        }

        /// <summary>
        /// Gets the connection string from configuration.
        /// </summary>
        /// <returns>A connection string.</returns>
        /// <exception cref="ConfigurationException">IF configuration is invalid or configuration section does not exist.</exception>
        public string GetConnectionString()
        {
            return configuration.GetValue(connectionStringKey, sectionKeys) ?? string.Empty;
        }
    }
}
