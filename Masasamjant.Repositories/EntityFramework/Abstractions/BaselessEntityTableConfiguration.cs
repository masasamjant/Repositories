using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masasamjant.Repositories.EntityFramework.Abstractions
{
    /// <summary>
    /// Represents <see cref="EntityTableConfiguration{TEntity}"/> where base type of <typeparamref name="TEntity"/> is removed from model.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class BaselessEntityTableConfiguration<TEntity> : EntityTableConfiguration<TEntity> where TEntity : class
    {
        /// <summary>
        /// Initializes new instance of the <see cref="BaselessEntityTableConfiguration{TEntity}"/> class.
        /// </summary>
        /// <param name="schemaName">The name of database schema.</param>
        /// <param name="tableName">The name of database table.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="schemaName"/> or <paramref name="tableName"/> is empty or contains only whitespace characters.</exception>
        protected BaselessEntityTableConfiguration(string schemaName, string tableName)
            : base(schemaName, tableName)
        { }

        /// <summary>
        /// Configures database schema for <typeparamref name="TEntity"/> entity without base type.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <remarks>Derived classes must call base implementation to remove base type from model.</remarks>
        protected override void ConfigureTableSchema(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasBaseType((Type?)null);
        }
    }
}
