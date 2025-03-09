using Masasamjant.Repositories.Abstractions;
using System.Linq.Expressions;

namespace Masasamjant.Repositories
{
    /// <summary>
    /// Represents basic implementation of <see cref="IQuerySpecification{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the queried class.</typeparam>
    public class QuerySpecification<T> : IQuerySpecification<T> where T : class
    {
        /// <summary>
        /// Initializes new instance of the <see cref="QuerySpecification{T}"/> class.
        /// </summary>
        /// <param name="criteria">The expression of query criteria.</param>
        public QuerySpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// Gets the expression of query criteria.
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; }
    }
}
