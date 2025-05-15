using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Masasamjant.Repositories
{
    internal sealed class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>, IQueryable
    {
        public AsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new AsyncQueryProvider(this);

        private class AsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> enumerator;

            public AsyncEnumerator(IEnumerator<T> enumerator)
            {
                this.enumerator = enumerator;
            }

            public T Current => enumerator.Current;

            public ValueTask DisposeAsync()
            {
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(enumerator.MoveNext());
            }
        }

        private class AsyncQueryProvider : IAsyncQueryProvider
        {
            private readonly IQueryProvider provider;

            public AsyncQueryProvider(IQueryProvider provider)
            {
                this.provider = provider;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return provider.CreateQuery(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return provider.CreateQuery<TElement>(expression);
            }

            public object? Execute(Expression expression)
            {
                return provider.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return provider.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                var result = Expression.Lambda(expression).Compile().DynamicInvoke();
                return Task.FromResult(result as dynamic);
            }
        }
    }
}
