namespace FinanceTracker.ImportExport.Importers;

/// <summary>
/// CSV importer uses a directory path containing three files:
/// accounts.csv, categories.csv, operations.csv with header rows.
/// </summary>
public class CsvImporter : DataImporter
{
    public CsvImporter(FinanceTracker.Domain.IDomainFactory factory) : base(factory) { }

    protected override ImportExport.DataBundleDto Load(string path)
    {
        var dto = new ImportExport.DataBundleDto();
        string accountsPath = Path.Combine(path, "accounts.csv");
        string categoriesPath = Path.Combine(path, "categories.csv");
        string operationsPath = Path.Combine(path, "operations.csv");

        if (File.Exists(accountsPath))
        {
            foreach (var line in File.ReadAllLines(accountsPath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = SplitCsv(line);
                dto.Accounts.Add(new ImportExport.AccountDto
                {
                    Id = Guid.Parse(parts[0]),
                    Name = parts[1],
                    Balance = decimal.Parse(parts[2])
                });
            }
        }

        if (File.Exists(categoriesPath))
        {
            foreach (var line in File.ReadAllLines(categoriesPath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = SplitCsv(line);
                dto.Categories.Add(new ImportExport.CategoryDto
                {
                    Id = Guid.Parse(parts[0]),
                    Name = parts[1],
                    Type = Enum.Parse<FinanceTracker.Domain.FinancialType>(parts[2], true)
                });
            }
        }

        if (File.Exists(operationsPath))
        {
            foreach (var line in File.ReadAllLines(operationsPath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = SplitCsv(line);
                dto.Operations.Add(new ImportExport.OperationDto
                {
                    Id = Guid.Parse(parts[0]),
                    Type = Enum.Parse<FinanceTracker.Domain.FinancialType>(parts[1], true),
                    BankAccountId = Guid.Parse(parts[2]),
                    CategoryId = Guid.Parse(parts[3]),
                    Amount = decimal.Parse(parts[4]),
                    Date = DateTime.Parse(parts[5]),
                    Description = parts.Length > 6 ? parts[6] : null
                });
            }
        }

        return dto;
    }

    private static string[] SplitCsv(string line)
    {
        // naive split that handles simple quoted fields
        var result = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();
        foreach (var ch in line)
        {
            if (ch == '\"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            if (ch == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else current.Append(ch);
        }
        result.Add(current.ToString());
        return result.ToArray();
    }
}
