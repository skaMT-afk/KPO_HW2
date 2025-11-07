namespace FinanceTracker.Commands;

public interface IAppCommand<out T>
{
    string Description { get; }
    T Execute();
}
