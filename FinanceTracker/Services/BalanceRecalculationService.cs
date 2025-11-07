using FinanceTracker.Domain;
using FinanceTracker.Repositories;

namespace FinanceTracker.Services;

public class BalanceRecalculationService
{
    private readonly IRepository<BankAccount> _accounts;
    private readonly IRepository<Operation> _ops;

    public BalanceRecalculationService(IRepository<BankAccount> accounts, IRepository<Operation> ops)
    {
        _accounts = accounts;
        _ops = ops;
    }

    public void RecalculateAccountBalance(Guid accountId)
    {
        var acc = _accounts.Get(accountId) ?? throw new InvalidOperationException("Account not found");
        var relatedOps = _ops.List().Where(o => o.BankAccountId == accountId);
        decimal balance = 0;
        foreach (var op in relatedOps)
        {
            balance += op.Type == Domain.FinancialType.Income ? op.Amount : -op.Amount;
        }
        // Keep the name, set recalculated balance
        var updated = new BankAccount(acc.Id, acc.Name, balance);
        _accounts.Update(updated);
    }
}
