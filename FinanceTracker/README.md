# FinanceTracker (Console, .NET 8, C#)

Домашняя работа №2: модуль «Учет финансов». Реализация с SOLID/GRASP и паттернами GoF.

## Что реализовано

### Доменная модель
- **BankAccount**: id, name, balance. Методы: `Rename`, внутренний `Apply` для изменения баланса.
- **Category**: id, name, type (Income/Expense).
- **Operation**: id, type, bank_account_id, category_id, amount, date, description.

Валидация и создание доменных объектов централизованы в **DomainFactory** (паттерн *Factory*). Например, сумма операции > 0, названия не пустые и т.п.

### Хранилище и Прокси
- Универсальный интерфейс `IStorage<T>` — «реальная БД». Реализация **FileStorage<T>** сохраняет в JSON-файлы (папка `Data/`).
- Интерфейс `IRepository<T>` — входная точка для приложения.
- **CachedRepositoryProxy<T>** реализует `IRepository<T>` и выступает *Proxy* над `IStorage<T>` с in-memory кэшем (read-through, write-through).

### Фасады
- **AccountFacade**, **CategoryFacade**, **OperationFacade** — CRUD-операции для каждой сущности + сценарии уровня предметной области (например, корректное обновление баланса при добавлении/удалении операций).
- **AnalyticsFacade** — аналитика: разница доходов/расходов за период и группировка по категориям.

### Управление данными
- **BalanceRecalculationService** — пересчет баланса счёта из операций (ручная процедура восстановления консистентности).

### Команда + Декоратор (измерение времени)
- Интерфейс `IAppCommand<T>` и конкретная команда **AddOperationCommand**.
- **TimingCommandDecorator<T>** оборачивает команду и записывает длительность в **StatsService** (*Decorator* над *Command*).

### Импорт (Шаблонный метод)
- Абстрактный **DataImporter** определяет общий шаблон импорта: `Import(path) -> Load(path) -> Map(...)` (*Template Method*).
- Реализации: **JsonImporter**, **YamlImporter**, **CsvImporter**.
  - JSON/YAML — один файл со всем набором данных.
  - CSV — директория с тремя файлами: `accounts.csv`, `categories.csv`, `operations.csv` (с заголовками).

### Экспорт (Посетитель)
- Интерфейс **IExportVisitor** и реализации: **JsonExportVisitor**, **YamlExportVisitor**, **CsvExportVisitor** (*Visitor*).
- **ExportService** обходит все объекты и вызывает `Accept(visitor)` у доменных классов.

### DI-контейнер
- Использована `Microsoft.Extensions.DependencyInjection` (+ пакет `YamlDotNet` для YAML).
- Регистрации: `IStorage<> -> FileStorage<>`, `IRepository<> -> CachedRepositoryProxy<>`, фасады/сервисы/фабрика как singletons.

### SOLID / GRASP
- **SRP**: фасады, сервисы и репозитории выполняют по одной ответственности.
- **OCP**: новые форматы импорта/экспорта добавляются классами без модификации существующих.
- **LSP**: прокси прозрачно подменяет репозиторий, сохраняя контракт.
- **ISP**: разделены интерфейсы `IStorage<T>`, `IRepository<T>`, `IAppCommand<T>`.
- **DIP**: зависящие компоненты получают абстракции через DI.
- **High Cohesion**: каждая часть содержит близкие по смыслу операции.
- **Low Coupling**: взаимодействие через интерфейсы и фасады.

## Запуск

1. Установите .NET 8 SDK.
2. В каталоге `FinanceTracker/` выполните:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
3. При первом запуске создаются файлы в `Data/`, а также экспорт в `Exports/`:
   - `data.json`, `data.yaml`, директория `csv/` с тремя CSV-файлами.

## Импорт/Экспорт вручную

- Экспорт выполняется автоматически при запуске через `ExportService` и посетителей.
- Для импорта можно создать небольшой фрагмент кода (пример):
   ```csharp
   var importer = new JsonImporter(new DomainFactory());
   var bundle = importer.Import("path/to/data.json");
   // далее можно сохранить bundle в репозитории (не включено в демо для краткости)
   ```

## Демонстрация в `Program.cs`
- Регистрируется DI, создаются фасады.
- При пустой БД создаются примерные данные.
- Добавление операции выполняется через команду, обёрнутую декоратором для измерения времени.
- Печатается аналитика и отчёт о среднем времени сценариев.
- Выполняется экспорт всеми тремя посетителями.
- Запускается процедура пересчёта баланса.



