using FinanceTracker.Domain;
using System.Collections.Concurrent;

namespace FinanceTracker.Repositories;

/// <summary>
/// Proxy with read-through cache over underlying storage (e.g., DB/file).
/// </summary>
public class CachedRepositoryProxy<T> : IRepository<T> where T : class, IEntity
{
    private readonly IStorage<T> _storage;
    private readonly ConcurrentDictionary<Guid, T> _cache = new();
    private bool _initialized;

    public CachedRepositoryProxy(IStorage<T> storage)
    {
        _storage = storage;
    }

    private void EnsureLoaded()
    {
        if (_initialized) return;
        foreach (var item in _storage.List())
            _cache[item.Id] = item;
        _initialized = true;
    }

    public T Add(T entity)
    {
        EnsureLoaded();
        _storage.Add(entity);
        _cache[entity.Id] = entity;
        return entity;
    }

    public void Delete(Guid id)
    {
        EnsureLoaded();
        _storage.Delete(id);
        _cache.TryRemove(id, out _);
    }

    public T? Get(Guid id)
    {
        EnsureLoaded();
        return _cache.TryGetValue(id, out var val) ? val : null;
    }

    public IEnumerable<T> List()
    {
        EnsureLoaded();
        return _cache.Values.ToList();
    }

    public void Update(T entity)
    {
        EnsureLoaded();
        _storage.Update(entity);
        _cache[entity.Id] = entity;
    }
}
