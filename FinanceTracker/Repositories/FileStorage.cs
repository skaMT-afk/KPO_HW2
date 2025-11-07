using System.Text.Json;
using FinanceTracker.Domain;

namespace FinanceTracker.Repositories;

public class FileStorage<T> : IStorage<T> where T : IEntity
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public FileStorage()
    {
        var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, $"{typeof(T).Name}.json");
    }

    private List<T> ReadAll()
    {
        lock (_lock)
        {
            if (!File.Exists(_filePath)) return new List<T>();
            var json = File.ReadAllText(_filePath);
            return string.IsNullOrWhiteSpace(json) ? new List<T>() :
                JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<T>();
        }
    }

    private void WriteAll(List<T> items)
    {
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }

    public T Add(T entity)
    {
        var items = ReadAll();
        items.Add(entity);
        WriteAll(items);
        return entity;
    }

    public void Delete(Guid id)
    {
        var items = ReadAll();
        items.RemoveAll(x => x.Id == id);
        WriteAll(items);
    }

    public T? Get(Guid id) => ReadAll().FirstOrDefault(x => x.Id == id);

    public IEnumerable<T> List() => ReadAll();

    public void Update(T entity)
    {
        var items = ReadAll();
        var idx = items.FindIndex(x => x.Id == entity.Id);
        if (idx >= 0) items[idx] = entity;
        WriteAll(items);
    }
}
