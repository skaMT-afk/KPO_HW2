using System.Text.Json.Serialization;

namespace FinanceTracker.Domain;

public class BankAccount : IEntity, IVisitable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Balance { get; private set; }

    [JsonConstructor]
    public BankAccount(Guid id, string name, decimal balance)
    {
        Id = id;
        Name = name;
        Balance = balance;
    }

    internal void Apply(decimal delta) => Balance += delta;

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Name cannot be empty.");
        Name = newName.Trim();
    }

    public void Accept(ImportExport.Export.IExportVisitor visitor) => visitor.Visit(this);
}
