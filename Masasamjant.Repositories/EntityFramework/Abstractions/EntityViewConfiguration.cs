using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masasamjant.Repositories.EntityFramework.Abstractions
{
    /// <summary>
    /// Represents base <see cref="EntityConfiguration{TEntity}"/> to configure <typeparamref name="TEntity"/> to database view.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class EntityViewConfiguration<TEntity> : EntityConfiguration<TEntity> where TEntity : class
    {
        /// <summary>
        /// Initializes new instance of the <see cref="EntityViewConfiguration{TEntity}"/> class.
        /// </summary>
        /// <param name="schemaName">The name of database schema.</param>
        /// <param name="viewName">The name of database view.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="schemaName"/> or <paramref name="viewName"/> is empty or contains only whitespace characters.</exception>
        protected EntityViewConfiguration(string schemaName, string viewName)
            : base(schemaName, viewName)
        { }

        /// <summary>
        /// Configures <typeparamref name="TEntity"/> type to database view.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        public sealed override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToView(ObjectName, SchemaName);
            ConfigureViewSchema(builder);
        }

        /// <summary>
        /// Configures database schema for <typeparamref name="TEntity"/> entity.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        protected abstract void ConfigureViewSchema(EntityTypeBuilder<TEntity> builder);
    }
}
