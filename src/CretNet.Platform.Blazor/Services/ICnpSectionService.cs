namespace CretNet.Platform.Blazor.Services;

public interface ICnpSectionService
{
    event Action? OnChange;
    void AddSection(object categoryId, object sectionId);
    void RemoveSection(object sectionId);
    IEnumerable<object> GetSections(object categoryId);
}