using FinanceTracker.Domain;

namespace FinanceTracker.Repositories;

public interface IRepository<T> where T : IEntity
{
    T Add(T entity);
    T? Get(Guid id);
    IEnumerable<T> List();
    void Update(T entity);
    void Delete(Guid id);
}

/// <summary>
/// Represents an underlying persistence (e.g., DB). Proxy will sit in front of this.
/// </summary>
public interface IStorage<T> where T : IEntity
{
    T Add(T entity);
    T? Get(Guid id);
    IEnumerable<T> List();
    void Update(T entity);
    void Delete(Guid id);
}
