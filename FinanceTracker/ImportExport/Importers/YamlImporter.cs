using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FinanceTracker.ImportExport.Importers;

public class YamlImporter : DataImporter
{
    public YamlImporter(FinanceTracker.Domain.IDomainFactory factory) : base(factory) { }

    protected override ImportExport.DataBundleDto Load(string path)
    {
        var yaml = File.ReadAllText(path);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<ImportExport.DataBundleDto>(yaml) ?? new ImportExport.DataBundleDto();
    }
}
