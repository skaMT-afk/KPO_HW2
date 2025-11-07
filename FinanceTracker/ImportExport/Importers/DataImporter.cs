using FinanceTracker.Domain;

namespace FinanceTracker.ImportExport.Importers;

public abstract class DataImporter
{
    protected readonly IDomainFactory Factory;

    protected DataImporter(IDomainFactory factory)
    {
        Factory = factory;
    }

    public DataBundle Import(string path)
    {
        var dto = Load(path);
        return Map(dto);
    }

    protected abstract ImportExport.DataBundleDto Load(string path);

    protected virtual DataBundle Map(ImportExport.DataBundleDto dto)
    {
        var bundle = new DataBundle();
        // ensure specified IDs are preserved (use constructors directly)
        foreach (var a in dto.Accounts)
            bundle.Accounts.Add(new Domain.BankAccount(a.Id == Guid.Empty ? Guid.NewGuid() : a.Id, a.Name, a.Balance));
        foreach (var c in dto.Categories)
            bundle.Categories.Add(new Domain.Category(c.Id == Guid.Empty ? Guid.NewGuid() : c.Id, c.Name, c.Type));
        foreach (var o in dto.Operations)
            bundle.Operations.Add(new Domain.Operation(o.Id == Guid.Empty ? Guid.NewGuid() : o.Id, o.Type, o.BankAccountId, o.CategoryId, o.Amount, o.Date, o.Description));
        return bundle;
    }
}
