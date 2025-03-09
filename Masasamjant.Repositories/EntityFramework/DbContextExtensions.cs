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
        /// Gets the query string of spcified criteria expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        /// <param name="expression">The criteria expression.</param>
        /// <returns>A query string or <c>null</c>.</returns>
        public static string? GetQueryString<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            try
            {
                var query = context.Set<TEntity>().Where(expression);
                var queryText = query.ToQueryString();
                return queryText;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
