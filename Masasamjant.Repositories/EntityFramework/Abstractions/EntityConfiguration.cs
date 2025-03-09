using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masasamjant.Repositories.EntityFramework.Abstractions
{
    /// <summary>
    /// Represents base implementation of <see cref="IEntityTypeConfiguration{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        /// <summary>
        /// Initializes new instance of the <see cref="EntityConfiguration{TEntity}"/> class.
        /// </summary>
        /// <param name="schemaName">The name of database schema.</param>
        /// <param name="objectName">The name of database object.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="schemaName"/> or <paramref name="objectName"/> is empty or contains only whitespace characters.</exception>
        protected EntityConfiguration(string schemaName, string objectName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                throw new ArgumentNullException(nameof(schemaName), "The schema name cannot be empty or only whitespace characters.");

            if (string.IsNullOrWhiteSpace(objectName))
                throw new ArgumentNullException(nameof(objectName), "The object name cannot be empty or only whitespace characters.");

            SchemaName = schemaName.Trim();
            ObjectName = objectName.Trim();
        }

        /// <summary>
        /// Gets the name of database schema.
        /// </summary>
        protected string SchemaName { get; }

        /// <summary>
        /// Gets the name of database object.
        /// </summary>
        protected string ObjectName { get; }

        /// <summary>
        /// Configures <typeparamref name="TEntity"/> type to database object.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
