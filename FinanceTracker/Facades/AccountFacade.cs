using FinanceTracker.Domain;
using FinanceTracker.Repositories;
using FinanceTracker.Services;

namespace FinanceTracker.Facades;

public class AccountFacade
{
    private readonly IRepository<BankAccount> _accounts;
    private readonly BalanceRecalculationService _recalc;

    public AccountFacade(IRepository<BankAccount> accounts, BalanceRecalculationService recalc)
    {
        _accounts = accounts;
        _recalc = recalc;
    }

    public BankAccount Create(string name, decimal initialBalance = 0m, IDomainFactory? factory = null)
    {
        factory ??= new DomainFactory();
        var acc = factory.CreateBankAccount(name, initialBalance);
        _accounts.Add(acc);
        return acc;
    }

    public IEnumerable<BankAccount> List() => _accounts.List();
    public BankAccount? Get(Guid id) => _accounts.Get(id);

    public void Rename(Guid id, string newName)
    {
        var acc = _accounts.Get(id) ?? throw new InvalidOperationException("Account not found");
        acc.Rename(newName);
        _accounts.Update(acc);
    }

    public void Delete(Guid id) => _accounts.Delete(id);

    public void Recalculate(Guid accountId) => _recalc.RecalculateAccountBalance(accountId);
}
