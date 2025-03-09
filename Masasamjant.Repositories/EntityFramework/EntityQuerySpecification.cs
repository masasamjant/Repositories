using System.Linq.Expressions;

namespace Masasamjant.Repositories.EntityFramework
{
    /// <summary>
    /// Represents <see cref="QuerySpecification{T}"/> for specify query for Entity Framework.
    /// </summary>
    /// <typeparam name="T">The type of the queried class.</typeparam>
    public class EntityQuerySpecification<T> : QuerySpecification<T> where T : class
    {
        /// <summary>
        /// Initializes new instance of the <see cref="EntityQuerySpecification{T}"/> class.
        /// </summary>
        /// <param name="criteria">The expression of query criteria.</param>
        public EntityQuerySpecification(Expression<Func<T, bool>> criteria)
            : this(criteria, false)
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="EntityQuerySpecification{T}"/> class.
        /// </summary>
        /// <param name="criteria">The expression of query criteria.</param>
        /// <param name="noTracking"><c>true</c> to perform no tracking query; <c>false</c> otherwise.</param>
        public EntityQuerySpecification(Expression<Func<T, bool>> criteria, bool noTracking)
            : base(criteria)
        {
            NoTracking = noTracking;
        }

        /// <summary>
        /// Gets whether or not perform no tracking query.
        /// </summary>
        public bool NoTracking { get; }
    }
}
