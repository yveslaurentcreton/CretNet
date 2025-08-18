using Ardalis.Specification;
using CretNet.Platform.Data.Abstractions;

namespace CretNet.Platform.Data;

public abstract class EntitySearchSpecification<T> : Specification<T>, IEntitySearchSpecification<T>
{
    private Action<string>? _searchAction;
    
    protected EntitySearchSpecification<T> DefineSearch(Action<string> searchAction)
    {
        _searchAction = searchAction;
        
        return this;
    }
    
    public void Configure(string searchTerm)
    {
        _searchAction?.Invoke(searchTerm);
    }
}