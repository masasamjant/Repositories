using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masasamjant.Repositories.EntityFramework.Abstractions
{
    /// <summary>
    /// Represents base <see cref="EntityConfiguration{TEntity}"/> to configure <typeparamref name="TEntity"/> to database table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class EntityTableConfiguration<TEntity> : EntityConfiguration<TEntity> where TEntity : class
    {
        /// <summary>
        /// Initializes new instance of the <see cref="EntityTableConfiguration{TEntity}"/> class.
        /// </summary>
        /// <param name="schemaName">The name of database schema.</param>
        /// <param name="tableName">The name of database table.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="schemaName"/> or <paramref name="tableName"/> is empty or contains only whitespace characters.</exception>
        protected EntityTableConfiguration(string schemaName, string tableName)
            : base(schemaName, tableName)
        { }

        /// <summary>
        /// Configures <typeparamref name="TEntity"/> type to database table.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        public sealed override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToTable(ObjectName, SchemaName);
            ConfigureTableSchema(builder);
        }

        /// <summary>
        /// Configures database schema for <typeparamref name="TEntity"/> entity.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        protected abstract void ConfigureTableSchema(EntityTypeBuilder<TEntity> builder);
    }
}
