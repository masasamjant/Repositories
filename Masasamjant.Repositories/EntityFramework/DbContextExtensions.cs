using Masasamjant.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Masasamjant.Repositories.EntityFramework
{
    /// <summary>
    /// Provides extension methods to <see cref="DbContext"/> class.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Gets the query string of specified criteria expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        /// <param name="expression">The criteria expression.</param>
        /// <returns>A query string or <c>null</c>.</returns>
        public static string? GetQueryString<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            try
            {
                return context.Set<TEntity>().GetQueryString(expression);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the query string of specified criteria expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="set">The <see cref="DbSet{TEntity}"/>.</param>
        /// <param name="expression">The criteria expression.</param>
        /// <returns>A query string or <c>null</c>.</returns>
        public static string? GetQueryString<TEntity>(this DbSet<TEntity> set, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            try
            {
                return set.Where(expression).ToQueryString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static string? GetQueryString<TEntity>(this IRepositoryEntries<TEntity> entries, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            try
            {
                return entries.Where(expression).ToQueryString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
