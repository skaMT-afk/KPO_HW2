using FinanceTracker.Domain;

namespace FinanceTracker.ImportExport.Export;

public interface IExportVisitor
{
    void Visit(BankAccount account);
    void Visit(Category category);
    void Visit(Operation operation);

    void SaveTo(string targetPath);
}
