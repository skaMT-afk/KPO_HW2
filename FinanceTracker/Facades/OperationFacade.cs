using FinanceTracker.Domain;
using FinanceTracker.Repositories;

namespace FinanceTracker.Facades;

public class OperationFacade
{
    private readonly IRepository<Operation> _ops;
    private readonly IRepository<BankAccount> _accounts;
    private readonly IRepository<Category> _categories;
    private readonly IDomainFactory _factory;

    public OperationFacade(IRepository<Operation> ops, IRepository<BankAccount> accounts, IRepository<Category> categories, IDomainFactory factory)
    {
        _ops = ops;
        _accounts = accounts;
        _categories = categories;
        _factory = factory;
    }

    public Operation Add(FinancialType type, Guid accountId, Guid categoryId, decimal amount, DateTime date, string? description = null)
    {
        var account = _accounts.Get(accountId) ?? throw new InvalidOperationException("Account not found");
        var category = _categories.Get(categoryId) ?? throw new InvalidOperationException("Category not found");
        if (category.Type != type) throw new InvalidOperationException("Category type mismatch with operation type.");

        var op = _factory.CreateOperation(type, accountId, categoryId, amount, date, description);
        _ops.Add(op);
        ApplyToAccount(account, op, +1);
        _accounts.Update(account);
        return op;
    }

    public void Delete(Guid operationId)
    {
        var op = _ops.Get(operationId) ?? throw new InvalidOperationException("Operation not found");
        var account = _accounts.Get(op.BankAccountId) ?? throw new InvalidOperationException("Account not found");
        ApplyToAccount(account, op, -1);
        _accounts.Update(account);
        _ops.Delete(operationId);
    }

    public void Update(Guid operationId, decimal amount, DateTime date, string? description, Guid categoryId, FinancialType type)
    {
        var op = _ops.Get(operationId) ?? throw new InvalidOperationException("Operation not found");
        var account = _accounts.Get(op.BankAccountId) ?? throw new InvalidOperationException("Account not found");
        // Revert old effect
        ApplyToAccount(account, op, -1);

        op.Update(amount, date, description, categoryId, type);
        _ops.Update(op);

        // Apply new effect
        ApplyToAccount(account, op, +1);
        _accounts.Update(account);
    }

    public IEnumerable<Operation> List() => _ops.List();
    public IEnumerable<Operation> ListByAccount(Guid accountId) => _ops.List().Where(o => o.BankAccountId == accountId);
    public IEnumerable<Operation> ListByPeriod(DateTime from, DateTime to) => _ops.List().Where(o => o.Date >= from && o.Date <= to);

    private static void ApplyToAccount(BankAccount account, Operation op, int sign)
    {
        var delta = op.Type == FinancialType.Income ? op.Amount : -op.Amount;
        account.Apply(sign * delta);
    }
}
