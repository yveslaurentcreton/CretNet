namespace CretNet.Platform.Blazor.Services
{
    public class CnpSectionService : ICnpSectionService
    {
        private Dictionary<object, object> _sections = [];

        public event Action? OnChange;

        public void AddSection(object categoryId, object sectionId)
        {
            _sections.Add(sectionId, categoryId);
            OnChange?.Invoke();
        }

        public void RemoveSection(object sectionId)
        {
            _sections.Remove(sectionId);
            OnChange?.Invoke();
        }
        
        public IEnumerable<object> GetSections(object categoryId)
        {
            return _sections.Where(x => x.Value == categoryId).Select(x => x.Key);
        }
    }
}
