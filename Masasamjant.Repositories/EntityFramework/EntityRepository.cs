using Masasamjant.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Masasamjant.Repositories.EntityFramework
{
    /// <summary>
    /// Represents base implementation of <see cref="IRepository"/> that use Entity Framework database provider.
    /// </summary>
    public class EntityRepository : DbContext, IRepository, IDisposable
    {
        private readonly IConnectionStringProvider connectionStringProvider;
        private EntityRepositoryTransaction? transaction;
        private readonly AutoResetEvent transactionResetEvent = new AutoResetEvent(true);

        /// <summary>
        /// Default value of <see cref="BeginTransactionTimeoutSeconds"/> is 60 seconds.
        /// </summary>
        protected const int DefaultTransactionTimeoutSeconds = 60;

        /// <summary>
        /// Initializes new instance of the <see cref="EntityRepository"/> class.
        /// </summary>
        /// <param name="connectionStringProvider">The <see cref="IConnectionStringProvider"/>.</param>
        protected EntityRepository(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// Finalizes current instance.
        /// </summary>
        ~EntityRepository()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets whether or not instance is disposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the timeout in seconds for attempt to create transaction.
        /// </summary>
        protected virtual int BeginTransactionTimeoutSeconds
        {
            get { return DefaultTransactionTimeoutSeconds; }
        }

        /// <summary>
        /// Add specified <typeparamref name="T"/> instance to repository.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The <typeparamref name="T"/> instance to add.</param>
        /// <param name="saveChanges"><c>true</c> to save changes immediately; <c>false</c> if <see cref="SaveChangesAsync"/> is invoked afterwards.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A added <typeparamref name="T"/> instance.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="ConcurrentUpdateException">If concurrent update error occurs.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        public async Task<T> AddAsync<T>(T instance, bool saveChanges = false, CancellationToken cancellationToken = default) where T : class
        {
            CheckDisposed();

            try
            {
                await OnAddAsync(instance, cancellationToken);

                var entry = await GetEntries<T>().AddAsync(instance, cancellationToken);

                if (saveChanges)
                    await SaveChangesAsync(cancellationToken);

                return entry.Entity;
            }
            catch (Exception exception)
            {
                if (exception is OperationCanceledException)
                    throw;
                else if (exception is ConcurrentUpdateException)
                    throw;
                else
                    throw RepositoryHelper.HandleRepositoryException(instance, "Add", null, exception);
            }
        }

        /// <summary>
        /// Disposes current instance.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Check if repository contains any instance of <typeparamref name="T"/> that match specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="specification">The criteria specification to match.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><c>true</c> if repository contains any instance of <typeparamref name="T"/> that match criteria of <paramref name="specification"/>; <c>false</c> otherwise.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        public async Task<bool> ExistsAsync<T>(IQuerySpecification<T> specification, CancellationToken cancellationToken = default) where T : class
        {
            CheckDisposed();

            var entries = GetEntries<T>();

            try
            {
                bool exists = await entries.AnyAsync(specification.Criteria, cancellationToken);

                return exists;
            }
            catch (Exception exception)
            {
                if (exception is OperationCanceledException)
                    throw;
                else 
                {
                    var queryText = entries.GetQueryString(specification.Criteria);
                    throw RepositoryHelper.HandleRepositoryException("Exists", queryText, exception);
                }
            }
        }

        /// <summary>
        /// Gets all instance of <typeparamref name="T"/> in repository. This method can be used in case the amount of instance of <typeparamref name="T"/> is limited 
        /// and it is known that all those are needed.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of <typeparamref name="T"/> instances in repository.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        public async Task<List<T>> GetAsync<T>(CancellationToken cancellationToken = default) where T : class
        {
            CheckDisposed();

            try
            {
                var result = await GetEntries<T>().ToListAsync(cancellationToken);

                return result;
            }
            catch (Exception exception)
            {
                if (exception is OperationCanceledException)
                    throw;
                else
                    throw RepositoryHelper.HandleRepositoryException("Get", null, exception);
            }
        }

        /// <summary>
        /// Gets all instance of <typeparamref name="T"/> in repository that match specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="specification">The criteria specification to match.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of <typeparamref name="T"/> instances in repository that match criteria of <paramref name="specification"/>.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        public async Task<List<T>> GetAsync<T>(IQuerySpecification<T> specification, CancellationToken cancellationToken = default) where T : class
        {
            CheckDisposed();

            var entitySpecification = GetEntityQuerySpecification(specification);

            try
            {
                List<T> result;

                if (entitySpecification.NoTracking)
                    result = await GetEntries<T>().Where(entitySpecification.Criteria).ToAsyncQueryable().AsNoTracking().ToListAsync(cancellationToken);
                else
                    result = await GetEntries<T>().Where(entitySpecification.Criteria).ToAsyncQueryable().ToListAsync(cancellationToken);

                return result;
            }
            catch (Exception exception)
            {
                if (exception is OperationCanceledException)
                    throw;
                else
                {
                    var queryText = DbContextExtensions.GetQueryString(this, entitySpecification.Criteria);
                    throw RepositoryHelper.HandleRepositoryException("Exists", null, exception);
                }
            }
        }

        /// <summary>
        /// Query instances of <typeparamref name="T"/> in repository.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>A query of <typeparamref name="T"/> instance in repository.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        public IQueryable<T> Query<T>() where T : class
        {
            CheckDisposed();

            try
            {
                return GetEntries<T>().ToAsyncQueryable();
            }
            catch (Exception exception)
            {
                throw RepositoryHelper.HandleRepositoryException("Query", null, exception);
            }
        }

        /// <summary>
        /// Query instances of <typeparamref name="T"/> in repository that match specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="specification">The criteria specification to match.</param>
        /// <returns>A query of <typeparamref name="T"/> instance in repository that match criteria of <paramref name="specification"/>.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        public IQueryable<T> Query<T>(IQuerySpecification<T> specification) where T : class
        {
            CheckDisposed();

            var entitySpecification = GetEntityQuerySpecification(specification);
            var entries = GetEntries<T>();

            try
            {
                if (entitySpecification.NoTracking)
                    return entries.Where(entitySpecification.Criteria).ToAsyncQueryable().AsNoTracking();
                else
                    return entries.Where(entitySpecification.Criteria).ToAsyncQueryable();
            }
            catch (Exception exception)
            {
                var queryText = DbContextExtensions.GetQueryString(entries, entitySpecification.Criteria);
                throw RepositoryHelper.HandleRepositoryException("Query", queryText, exception);
            }
        }

        /// <summary>
        /// Remove, delete permanently, instance of <typeparamref name="T"/> from repository.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The instance to remove.</param>
        /// <param name="saveChanges"><c>true</c> to save changes immediately; <c>false</c> if <see cref="SaveChangesAsync"/> is invoked afterwards.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A removed instance.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="ConcurrentUpdateException">If concurrent update error occurs.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        public async Task<T> RemoveAsync<T>(T instance, bool saveChanges = false, CancellationToken cancellationToken = default) where T : class
        {
            CheckDisposed();

            try
            {
                await OnRemoveAsync(instance, cancellationToken);

                var entry = GetEntries<T>().Remove(instance);

                if (saveChanges)
                    await SaveChangesAsync(cancellationToken);

                return entry.Entity;
            }
            catch (Exception exception)
            {
                if (exception is ConcurrentUpdateException)
                    throw;
                else if (exception is OperationCanceledException)
                    throw;
                else
                    throw RepositoryHelper.HandleRepositoryException(instance, "Remove", null, exception);
            }
        }

        /// <summary>
        /// Save changes made to instance stored in repository.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A count of changed instances.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="ConcurrentUpdateException">If concurrent update error occurs.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            
            try
            {
                return await DoSaveChangesAsync(true, cancellationToken);
            }
            catch (DbUpdateConcurrencyException exception)
            {
                throw await RepositoryHelper.HandleConcurrentUpdateExceptionAsync(exception);
            }
            catch (Exception exception)
            {
                if (exception is ConcurrentUpdateException)
                    throw;
                else if (exception is OperationCanceledException)
                    throw;
                else
                    throw RepositoryHelper.HandleRepositoryException("SaveChanges", null, exception);
            }
        }

        /// <summary>
        /// Update specified instance of <typeparamref name="T"/> in repository.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The instance to update.</param>
        /// <param name="saveChanges"><c>true</c> to save changes immediately; <c>false</c> if <see cref="SaveChangesAsync"/> is invoked afterwards.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A updated instance.</returns>
        /// <exception cref="RepositoryException">If exception occurs at operation.</exception>
        /// <exception cref="ConcurrentUpdateException">If concurrent update error occurs.</exception>
        /// <exception cref="OperationCanceledException">If operation is cancelled.</exception>
        public async Task<T> UpdateAsync<T>(T instance, bool saveChanges = false, CancellationToken cancellationToken = default) where T : class
        {
            CheckDisposed();

            try
            {
                await OnUpdateAsync(instance, cancellationToken);

                var entry = GetEntries<T>().Update(instance);

                if (saveChanges)
                    await SaveChangesAsync(cancellationToken);

                return entry.Entity;
            }
            catch (Exception exception)
            {
                if (exception is ConcurrentUpdateException)
                    throw;
                else if (exception is OperationCanceledException)
                    throw;
                else
                    throw RepositoryHelper.HandleRepositoryException(instance, "Update", null, exception);
            }
        }

        /// <summary>
        /// Begins new transaction. Repository supports only one transaction at a time.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">If transactions are not supported.</exception>
        /// <exception cref="InvalidOperationException">If cannot begin new transaction.</exception>
        public virtual async Task<IRepositoryTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            int transactionTimeout = BeginTransactionTimeoutSeconds;

            if (transactionTimeout < 1)
                transactionTimeout = DefaultTransactionTimeoutSeconds;

            bool raiseResetEvent = false;

            try
            {
                transactionResetEvent.WaitOne(TimeSpan.FromSeconds(transactionTimeout));
                raiseResetEvent = true;

                CheckCurrentTransaction();
                var contextTransaction = await Database.BeginTransactionAsync(cancellationToken);
                transaction = new EntityRepositoryTransaction(contextTransaction, transactionTimeout);
                return transaction;
            }
            catch (AbandonedMutexException exception)
            {
                throw new InvalidOperationException($"Transaction creation timeouted in {transactionTimeout} seconds.", exception);
            }
            finally
            {
                if (raiseResetEvent)
                    transactionResetEvent.Set();
            }
        }



        /// <summary>
        /// Check if <see cref="IsDisposed"/> is <c>true</c> and if so, then throws <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If instance has been disposed.</exception>
        protected void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <summary>
        /// Disposes current instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> if disposing; <c>false</c> otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            if (disposing)
                base.Dispose();
        }

        /// <summary>
        /// Gets global timeout in seconds for executed commands or <c>null</c>, default, to use default timeout.
        /// </summary>
        /// <returns>A command timeout in seconds or <c>null</c>.</returns>
        protected virtual int? GetCommandTimeoutMilliseconds() => null;

        /// <summary>
        /// Configures context.
        /// </summary>
        /// <param name="optionsBuilder">The <see cref="DbContextOptionsBuilder"/>.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = connectionStringProvider.GetConnectionString();
            var commandTimeout = GetCommandTimeoutMilliseconds();

            if (commandTimeout.HasValue && commandTimeout.Value > 0)
                optionsBuilder.UseSqlServer(connectionString, op => op.CommandTimeout(commandTimeout));
            else
                optionsBuilder.UseSqlServer(connectionString);
        }

        /// <summary>
        /// Gets the entries of <typeparamref name="T"/> in repository.
        /// </summary>
        /// <typeparam name="T">The type the item.</typeparam>
        /// <returns>A <see cref="IRepositoryEntries{T}"/> of <typeparamref name="T"/>.</returns>
        protected virtual IRepositoryEntries<T> GetEntries<T>() where T : class
            => new EntityRepositoryEntries<T>(Set<T>());

        /// <summary>
        /// Save changes made to entities.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"><c>true</c> to accept all changes when success; <c>false</c> otherwise.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A count of affected entities.</returns>
        protected virtual Task<int> DoSaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
            => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        /// <summary>
        /// Invoked before <typeparamref name="T"/> instance is added to context.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The <typeparamref name="T"/> instance about to be added.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        protected virtual Task OnAddAsync<T>(T instance, CancellationToken cancellationToken) where T : class
            => Task.CompletedTask;

        /// <summary>
        /// Invoked before <typeparamref name="T"/> instance is removed from context.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The <typeparamref name="T"/> instance about to be removed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        protected virtual Task OnRemoveAsync<T>(T instance, CancellationToken cancellationToken) where T : class
            => Task.CompletedTask;

        /// <summary>
        /// Invoked before <typeparamref name="T"/> instance is updated at context.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The <typeparamref name="T"/> instance about to be updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        protected virtual Task OnUpdateAsync<T>(T instance, CancellationToken cancellationToken) where T : class
            => Task.CompletedTask;

        private static EntityQuerySpecification<T> GetEntityQuerySpecification<T>(IQuerySpecification<T> specification) where T : class
        {
            if (specification is EntityQuerySpecification<T> entityQuerySpecification)
                return entityQuerySpecification;

            return new EntityQuerySpecification<T>(specification.Criteria);
        }

        private void CheckCurrentTransaction()
        {
            if (transaction != null)
            {
                if (transaction.TransactionState == RepositoryTransactionState.Uncommitted)
                    throw new InvalidOperationException("Transaction already exists. Commit or rollback current transaction before creating new one.");
                else
                {
                    if (!transaction.IsDisposed)
                        transaction.Dispose();

                    transaction = null;
                }
            }
        }
    }
}
