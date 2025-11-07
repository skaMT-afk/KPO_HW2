using FinanceTracker.Commands;
using FinanceTracker.Domain;
using FinanceTracker.Facades;
using FinanceTracker.ImportExport.Export;
using FinanceTracker.ImportExport.Importers;
using FinanceTracker.Repositories;
using FinanceTracker.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// DI registrations
services.AddSingleton(typeof(IStorage<>), typeof(FileStorage<>));            // "DB"
services.AddSingleton(typeof(IRepository<>), typeof(CachedRepositoryProxy<>)); // Proxy + cache

services.AddSingleton<BalanceRecalculationService>();
services.AddSingleton<StatsService>();

services.AddSingleton<IDomainFactory, DomainFactory>();
services.AddSingleton<AccountFacade>();
services.AddSingleton<CategoryFacade>();
services.AddSingleton<OperationFacade>();
services.AddSingleton<AnalyticsFacade>();

services.AddSingleton<ExportService>();

services.AddSingleton<DataImporter, JsonImporter>(); // default importer (can resolve specific ones manually)

var provider = services.BuildServiceProvider();

// Resolve facades/services
var accounts = provider.GetRequiredService<AccountFacade>();
var categories = provider.GetRequiredService<CategoryFacade>();
var ops = provider.GetRequiredService<OperationFacade>();
var analytics = provider.GetRequiredService<AnalyticsFacade>();
var stats = provider.GetRequiredService<StatsService>();
var factory = provider.GetRequiredService<IDomainFactory>();

// Seed minimal data if empty
if (!accounts.List().Any())
{
    var acc = accounts.Create("Основной счет", 0m, factory);
    var catSalary = categories.Create("Зарплата", FinancialType.Income, factory);
    var catCafe = categories.Create("Кафе", FinancialType.Expense, factory);

    // Command + Decorator (timing)
    var cmd1 = new AddOperationCommand(ops, FinancialType.Income, acc.Id, catSalary.Id, 120_000m, DateTime.Today.AddDays(-3), "Октябрьская зарплата");
    var timed1 = new TimingCommandDecorator<FinanceTracker.Domain.Operation>(cmd1, stats);
    timed1.Execute();

    var cmd2 = new AddOperationCommand(ops, FinancialType.Expense, acc.Id, catCafe.Id, 850m, DateTime.Today.AddDays(-2), "Капучино");
    var timed2 = new TimingCommandDecorator<FinanceTracker.Domain.Operation>(cmd2, stats);
    timed2.Execute();
}

// Show analytics
var (inc, exp, net) = analytics.IncomeVsExpense(DateTime.Today.AddDays(-30), DateTime.Today);
Console.WriteLine($"Доходы: {inc}, Расходы: {exp}, Итог: {net}");

foreach (var grp in analytics.GroupByCategory(DateTime.Today.AddDays(-30), DateTime.Today))
{
    Console.WriteLine($"Категория: {grp.CategoryName} ({grp.Type}) = {grp.TotalAmount}");
}

// Export examples using Visitor
var export = provider.GetRequiredService<ExportService>();
var exportDir = Path.Combine(AppContext.BaseDirectory, "Exports");
Directory.CreateDirectory(exportDir);

export.Export(new JsonExportVisitor(), Path.Combine(exportDir, "data.json"));
export.Export(new YamlExportVisitor(), Path.Combine(exportDir, "data.yaml"));
export.Export(new CsvExportVisitor(), Path.Combine(exportDir, "csv"));

// Recalculate example
var firstAcc = accounts.List().First();
accounts.Recalculate(firstAcc.Id);

Console.WriteLine("\nСтатистика сценариев:");
Console.WriteLine(stats.Report());

Console.WriteLine("\nГотово. Файлы экспорта записаны в папку 'Exports' рядом с исполняемым файлом.");
