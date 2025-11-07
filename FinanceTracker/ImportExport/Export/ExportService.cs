using FinanceTracker.Domain;
using FinanceTracker.Repositories;

namespace FinanceTracker.ImportExport.Export;

public class ExportService
{
    private readonly IRepository<BankAccount> _accounts;
    private readonly IRepository<Category> _categories;
    private readonly IRepository<Operation> _operations;

    public ExportService(IRepository<BankAccount> accounts, IRepository<Category> categories, IRepository<Operation> operations)
    {
        _accounts = accounts;
        _categories = categories;
        _operations = operations;
    }

    public void Export(IExportVisitor visitor, string targetPath)
    {
        foreach (var a in _accounts.List()) a.Accept(visitor);
        foreach (var c in _categories.List()) c.Accept(visitor);
        foreach (var o in _operations.List()) o.Accept(visitor);
        visitor.SaveTo(targetPath);
    }
}
