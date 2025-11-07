using System.Diagnostics;
using FinanceTracker.Services;

namespace FinanceTracker.Commands;

public class TimingCommandDecorator<T> : IAppCommand<T>
{
    private readonly IAppCommand<T> _inner;
    private readonly StatsService _stats;

    public TimingCommandDecorator(IAppCommand<T> inner, StatsService stats)
    {
        _inner = inner;
        _stats = stats;
    }

    public string Description => _inner.Description;

    public T Execute()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return _inner.Execute();
        }
        finally
        {
            sw.Stop();
            _stats.Record(Description, sw.Elapsed);
        }
    }
}
