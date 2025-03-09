using System.Linq.Expressions;

namespace Masasamjant.Repositories.Abstractions
{
    /// <summary>
    /// Represents specification of an query.
    /// </summary>
    /// <typeparam name="T">The type of the queried class.</typeparam>
    public interface IQuerySpecification<T> where T : class
    {
        /// <summary>
        /// Gets the expression of query criteria.
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }
    }
}
