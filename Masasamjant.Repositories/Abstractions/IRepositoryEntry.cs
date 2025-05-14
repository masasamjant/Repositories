namespace Masasamjant.Repositories.Abstractions
{
    /// <summary>
    /// Represents an entry of <typeparamref name="T"/> in repository.
    /// </summary>
    /// <typeparam name="T">The type of the entry item.</typeparam>
    public interface IRepositoryEntry<T> where T : class
    {
        /// <summary>
        /// Gets the entry item.
        /// </summary>
        T Entity { get; }
    }
}
