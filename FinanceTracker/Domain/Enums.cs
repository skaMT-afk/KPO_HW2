namespace FinanceTracker.Domain;

public enum FinancialType
{
    Income = 1,
    Expense = 2
}

public interface IEntity
{
    Guid Id { get; }
}

public interface IVisitable
{
    void Accept(ImportExport.Export.IExportVisitor visitor);
}
