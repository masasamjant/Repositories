using Microsoft.Extensions.Configuration;

namespace Masasamjant.Repositories.Configuration
{
    /// <summary>
    /// Provides extension methods to <see cref="IConfiguration"/> interface.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets configuration value specified by key at configuration section specified by section key.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="key">The configuration key.</param>
        /// <param name="sectionKey">The configuration section key.</param>
        /// <returns>A configuration value.</returns>
        /// <exception cref="ConfigurationException">If configuration contains errors or section does not exist.</exception>
        public static string? GetValue(this IConfiguration configuration, string key, string sectionKey)
        {
            try
            {
                var configurationSection = configuration.GetRequiredSection(sectionKey);
                return configurationSection[key];
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(key, [sectionKey], "The configuration contains errors or configuration section does not exist.", exception);
            }
        }

        /// <summary>
        /// Gets configuration value specified by key either from configuration or from configuration section specified by section name.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="key">The configuration keys.</param>
        /// <param name="sectionKeys">The configuration section keys.</param>
        /// <returns>A configuration value.</returns>
        /// <exception cref="ConfigurationException">If configuration contains errors, section key is invalid or section does not exist.</exception>
        public static string? GetValue(this IConfiguration configuration, string key, IEnumerable<string>? sectionKeys = null)
        {
            try
            {
                if (sectionKeys != null && sectionKeys.Any())
                {
                    IConfigurationSection? configurationSection = null;

                    foreach (var sectionKey in sectionKeys)
                    {
                        if (string.IsNullOrWhiteSpace(sectionKey))
                            throw new InvalidOperationException("The section key is empty or contains only whitespace characters.");

                        configurationSection = configurationSection != null
                            ? configurationSection.GetRequiredSection(sectionKey)
                            : configuration.GetRequiredSection(sectionKey);
                    }

                    if (configurationSection == null)
                        throw new InvalidOperationException("The configuration section could not be resolved.");

                    return configurationSection[key];
                }
                else
                    return configuration[key];
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(key, sectionKeys ?? [], "The configuration contains errors or configuration section does not exist.", exception);
            }
        }
    }
}
