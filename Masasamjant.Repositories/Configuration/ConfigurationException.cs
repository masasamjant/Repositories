namespace Masasamjant.Repositories.Configuration
{
    /// <summary>
    /// Represents exception thrown when configuration has errors.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Initializes new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        public ConfigurationException(string configurationKey)
            : this(configurationKey, "The configuration contains errors.")
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="message">The exception message.</param>
        public ConfigurationException(string configurationKey, string message)
            : this(configurationKey, [], message)
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="configurationSections">The configuration section names.</param>
        public ConfigurationException(string configurationKey, IEnumerable<string> configurationSections)
            : this(configurationKey, configurationSections, "The configuration contains errors.")
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="configurationSections">The configuration section names.</param>
        /// <param name="message">The exception message.</param>
        public ConfigurationException(string configurationKey, IEnumerable<string> configurationSections, string message)
            : this(configurationKey, configurationSections, message, null)
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="configurationSections">The configuration section names.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception or <c>null</c>.</param>
        public ConfigurationException(string configurationKey, IEnumerable<string> configurationSections, string message, Exception? innerException)
            : base(message, innerException)
        {
            ConfigurationKey = configurationKey;
            ConfigurationSections = configurationSections;
        }

        /// <summary>
        /// Gets the configuration key.
        /// </summary>
        public string ConfigurationKey { get; }

        /// <summary>
        /// Gets the configuration section names.
        /// </summary>
        public IEnumerable<string> ConfigurationSections { get; }
    }
}
