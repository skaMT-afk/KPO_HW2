using FinanceTracker.Domain;
using FinanceTracker.Repositories;

namespace FinanceTracker.Facades;

public record CategorySummary(string CategoryName, FinancialType Type, decimal TotalAmount);

public class AnalyticsFacade
{
    private readonly IRepository<Operation> _ops;
    private readonly IRepository<Category> _cats;

    public AnalyticsFacade(IRepository<Operation> ops, IRepository<Category> cats)
    {
        _ops = ops;
        _cats = cats;
    }

    public (decimal income, decimal expense, decimal net) IncomeVsExpense(DateTime from, DateTime to)
    {
        var query = _ops.List().Where(o => o.Date >= from && o.Date <= to);
        var income = query.Where(o => o.Type == FinancialType.Income).Sum(o => o.Amount);
        var expense = query.Where(o => o.Type == FinancialType.Expense).Sum(o => o.Amount);
        return (income, expense, income - expense);
    }

    public IEnumerable<CategorySummary> GroupByCategory(DateTime from, DateTime to)
    {
        var ops = _ops.List().Where(o => o.Date >= from && o.Date <= to)
            .GroupBy(o => o.CategoryId);

        foreach (var g in ops)
        {
            var cat = _cats.Get(g.Key);
            var name = cat?.Name ?? "Unknown";
            var type = cat?.Type ?? FinancialType.Expense;
            yield return new CategorySummary(name, type, g.Sum(o => o.Amount));
        }
    }
}
