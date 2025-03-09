using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masasamjant.Repositories.EntityFramework.Abstractions
{
    /// <summary>
    /// Represents <see cref="EntityViewConfiguration{TEntity}"/> where base type of <typeparamref name="TEntity"/> is removed from model.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class BaselessEntityViewConfiguration<TEntity> : EntityViewConfiguration<TEntity> where TEntity : class
    {
        /// <summary>
        /// Initializes new instance of the <see cref="BaselessEntityViewConfiguration{TEntity}"/> class.
        /// </summary>
        /// <param name="schemaName">The name of database schema.</param>
        /// <param name="viewName">The name of database view.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="schemaName"/> or <paramref name="viewName"/> is empty or contains only whitespace characters.</exception>
        protected BaselessEntityViewConfiguration(string schemaName, string viewName)
            : base(schemaName, viewName)
        { }

        /// <summary>
        /// Configures database schema for <typeparamref name="TEntity"/> entity without base type.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <remarks>Derived classes must call base implementation to remove base type from model.</remarks>
        protected override void ConfigureViewSchema(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasBaseType((Type?)null);
        }
    }
}
