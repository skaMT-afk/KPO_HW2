using FinanceTracker.Domain;
using System.Text;

namespace FinanceTracker.ImportExport.Export;

public class CsvExportVisitor : IExportVisitor
{
    private readonly List<BankAccount> _accounts = new();
    private readonly List<Category> _categories = new();
    private readonly List<Operation> _operations = new();

    public void Visit(BankAccount account) => _accounts.Add(account);
    public void Visit(Category category) => _categories.Add(category);
    public void Visit(Operation operation) => _operations.Add(operation);

    public void SaveTo(string targetPath)
    {
        // targetPath is directory for CSV export
        Directory.CreateDirectory(targetPath);

        File.WriteAllText(Path.Combine(targetPath, "accounts.csv"),
            ToCsv(new[] { "Id", "Name", "Balance" },
                _accounts.Select(a => new[] { a.Id.ToString(), a.Name, a.Balance.ToString() })));

        File.WriteAllText(Path.Combine(targetPath, "categories.csv"),
            ToCsv(new[] { "Id", "Name", "Type" },
                _categories.Select(c => new[] { c.Id.ToString(), c.Name, c.Type.ToString() })));

        File.WriteAllText(Path.Combine(targetPath, "operations.csv"),
            ToCsv(new[] { "Id", "Type", "BankAccountId", "CategoryId", "Amount", "Date", "Description" },
                _operations.Select(o => new[] { o.Id.ToString(), o.Type.ToString(), o.BankAccountId.ToString(), o.CategoryId.ToString(), o.Amount.ToString(), o.Date.ToString("u"), o.Description ?? "" })));
    }

    private static string ToCsv(IEnumerable<string> header, IEnumerable<IEnumerable<string>> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", header));
        foreach (var r in rows)
        {
            sb.AppendLine(string.Join(",", r.Select(s => s.Contains(',') || s.Contains('\"') ? $"\"{s.Replace("\"", "\"\"")}\"" : s)));
        }
        return sb.ToString();
    }
}
