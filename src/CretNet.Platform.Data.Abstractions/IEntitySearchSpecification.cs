using Ardalis.Specification;

namespace CretNet.Platform.Data.Abstractions;

public interface IEntitySearchSpecification<T> : ISpecification<T>
{
    void Configure(string searchTerm);
}