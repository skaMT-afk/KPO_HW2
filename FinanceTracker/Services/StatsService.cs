using System.Collections.Concurrent;

namespace FinanceTracker.Services;

public class StatsService
{
    private readonly ConcurrentDictionary<string, List<TimeSpan>> _durations = new();

    public void Record(string scenario, TimeSpan duration)
    {
        var list = _durations.GetOrAdd(scenario, _ => new List<TimeSpan>());
        lock (list) list.Add(duration);
    }

    public string Report()
    {
        var lines = new List<string>();
        foreach (var kvp in _durations.OrderBy(k => k.Key))
        {
            var avg = TimeSpan.FromMilliseconds(kvp.Value.Average(t => t.TotalMilliseconds));
            lines.Add($"{kvp.Key}: count={kvp.Value.Count}, avg={avg.TotalMilliseconds:F2} ms");
        }
        return string.Join(Environment.NewLine, lines);
    }
}
