using System.Text.Json.Serialization;

namespace FinanceTracker.Domain;

public class Category : IEntity, IVisitable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public FinancialType Type { get; private set; }

    [JsonConstructor]
    public Category(Guid id, string name, FinancialType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Name cannot be empty.");
        Name = newName.Trim();
    }

    public void Accept(ImportExport.Export.IExportVisitor visitor) => visitor.Visit(this);
}
