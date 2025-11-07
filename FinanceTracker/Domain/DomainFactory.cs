namespace FinanceTracker.Domain;

public interface IDomainFactory
{
    BankAccount CreateBankAccount(string name, decimal initialBalance = 0m);
    Category CreateCategory(string name, FinancialType type);
    Operation CreateOperation(FinancialType type, Guid accountId, Guid categoryId, decimal amount, DateTime date, string? description = null);
}

public class DomainFactory : IDomainFactory
{
    public BankAccount CreateBankAccount(string name, decimal initialBalance = 0m)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Account name required.");
        if (initialBalance < 0) throw new ArgumentException("Initial balance cannot be negative.");
        return new BankAccount(Guid.NewGuid(), name.Trim(), initialBalance);
    }

    public Category CreateCategory(string name, FinancialType type)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Category name required.");
        return new Category(Guid.NewGuid(), name.Trim(), type);
    }

    public Operation CreateOperation(FinancialType type, Guid accountId, Guid categoryId, decimal amount, DateTime date, string? description = null)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        if (accountId == Guid.Empty) throw new ArgumentException("Account required.");
        if (categoryId == Guid.Empty) throw new ArgumentException("Category required.");
        return new Operation(Guid.NewGuid(), type, accountId, categoryId, amount, date, description);
    }
}
