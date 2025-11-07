using FinanceTracker.Domain;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FinanceTracker.ImportExport.Export;

public class YamlExportVisitor : IExportVisitor
{
    private readonly List<BankAccount> _accounts = new();
    private readonly List<Category> _categories = new();
    private readonly List<Operation> _operations = new();

    public void Visit(BankAccount account) => _accounts.Add(account);
    public void Visit(Category category) => _categories.Add(category);
    public void Visit(Operation operation) => _operations.Add(operation);

    public void SaveTo(string targetPath)
    {
        var bundle = new ImportExport.DataBundleDto
        {
            Accounts = _accounts.Select(a => new ImportExport.AccountDto { Id = a.Id, Name = a.Name, Balance = a.Balance }).ToList(),
            Categories = _categories.Select(c => new ImportExport.CategoryDto { Id = c.Id, Name = c.Name, Type = c.Type }).ToList(),
            Operations = _operations.Select(o => new ImportExport.OperationDto { Id = o.Id, Type = o.Type, BankAccountId = o.BankAccountId, CategoryId = o.CategoryId, Amount = o.Amount, Date = o.Date, Description = o.Description }).ToList()
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(bundle);
        File.WriteAllText(targetPath, yaml);
    }
}
