using System.Text.Json;

namespace FinanceTracker.ImportExport.Importers;

public class JsonImporter : DataImporter
{
    public JsonImporter(FinanceTracker.Domain.IDomainFactory factory) : base(factory) { }

    protected override ImportExport.DataBundleDto Load(string path)
    {
        var json = File.ReadAllText(path);
        var dto = JsonSerializer.Deserialize<ImportExport.DataBundleDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return dto ?? new ImportExport.DataBundleDto();
    }
}
