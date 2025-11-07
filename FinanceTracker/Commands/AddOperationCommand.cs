using FinanceTracker.Domain;
using FinanceTracker.Facades;

namespace FinanceTracker.Commands;

public class AddOperationCommand : IAppCommand<Domain.Operation>
{
    private readonly OperationFacade _ops;
    private readonly FinancialType _type;
    private readonly Guid _accountId;
    private readonly Guid _categoryId;
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly string? _desc;

    public AddOperationCommand(OperationFacade ops, FinancialType type, Guid accountId, Guid categoryId, decimal amount, DateTime date, string? description)
    {
        _ops = ops;
        _type = type;
        _accountId = accountId;
        _categoryId = categoryId;
        _amount = amount;
        _date = date;
        _desc = description;
    }

    public string Description => "AddOperation";

    public Domain.Operation Execute() => _ops.Add(_type, _accountId, _categoryId, _amount, _date, _desc);
}
