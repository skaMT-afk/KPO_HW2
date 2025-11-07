using FinanceTracker.Domain;

namespace FinanceTracker.ImportExport;

public class DataBundle
{
    public List<BankAccount> Accounts { get; } = new();
    public List<Category> Categories { get; } = new();
    public List<Operation> Operations { get; } = new();
}

// DTOs for importers (parsing stage)
public class DataBundleDto
{
    public List<AccountDto> Accounts { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
    public List<OperationDto> Operations { get; set; } = new();
}

public class AccountDto { public Guid Id { get; set; } public string Name { get; set; } = ""; public decimal Balance { get; set; } }
public class CategoryDto { public Guid Id { get; set; } public string Name { get; set; } = ""; public FinancialType Type { get; set; } }
public class OperationDto
{
    public Guid Id { get; set; }
    public FinancialType Type { get; set; }
    public Guid BankAccountId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
