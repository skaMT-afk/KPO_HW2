using System.Text.Json.Serialization;

namespace FinanceTracker.Domain;

public class Operation : IEntity, IVisitable
{
    public Guid Id { get; private set; }
    public FinancialType Type { get; private set; }
    public Guid BankAccountId { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string? Description { get; private set; }

    [JsonConstructor]
    public Operation(Guid id, FinancialType type, Guid bankAccountId, Guid categoryId, decimal amount, DateTime date, string? description)
    {
        Id = id;
        Type = type;
        BankAccountId = bankAccountId;
        CategoryId = categoryId;
        Amount = amount;
        Date = date;
        Description = description;
    }

    public void Update(decimal amount, DateTime date, string? description, Guid categoryId, FinancialType type)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        Amount = amount;
        Date = date;
        Description = description;
        CategoryId = categoryId;
        Type = type;
    }

    public void Accept(ImportExport.Export.IExportVisitor visitor) => visitor.Visit(this);
}
