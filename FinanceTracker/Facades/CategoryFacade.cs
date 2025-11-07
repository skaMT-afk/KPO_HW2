using FinanceTracker.Domain;
using FinanceTracker.Repositories;

namespace FinanceTracker.Facades;

public class CategoryFacade
{
    private readonly IRepository<Category> _categories;

    public CategoryFacade(IRepository<Category> categories)
    {
        _categories = categories;
    }

    public Category Create(string name, FinancialType type, IDomainFactory? factory = null)
    {
        factory ??= new DomainFactory();
        var cat = factory.CreateCategory(name, type);
        _categories.Add(cat);
        return cat;
    }

    public IEnumerable<Category> List() => _categories.List();

    public Category? Get(Guid id) => _categories.Get(id);

    public void Rename(Guid id, string newName)
    {
        var cat = _categories.Get(id) ?? throw new InvalidOperationException("Category not found");
        cat.Rename(newName);
        _categories.Update(cat);
    }

    public void Delete(Guid id) => _categories.Delete(id);
}
