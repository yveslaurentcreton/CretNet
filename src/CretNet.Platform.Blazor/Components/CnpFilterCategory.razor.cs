using CretNet.Platform.Blazor.Models;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpFilterCategory
{
    private void OnAllValuesCheckedChangedInternalAsync(bool? newAreAllVisible)
    {
        SetCheckState(newAreAllVisible, Values);
    }

    private void OnValueVisibilityChangedInternalAsync(IEntityFilter filter, bool isVisible)
    {
        filter.Enabled = isVisible;
    }

    private static void SetCheckState(bool? newAreAllVisible, IEnumerable<IEntityFilter> values)
    {
        if (newAreAllVisible is null)
        {
            return;
        }

        foreach (var value in values)
        {
            value.Enabled = newAreAllVisible.Value;
        }
    }

    private static bool? GetCheckState(IEnumerable<IEntityFilter> filtersEnumerable)
    {
        var filters = filtersEnumerable.ToList();
        
        if (filters.Count == 0)
            return true;

        var areAllChecked = true;
        var areAllUnchecked = true;

        foreach (var filter in filters)
        {
            if (filter.Enabled)
            {
                areAllUnchecked = false;
            }
            else
            {
                areAllChecked = false;
            }
        }

        if (areAllChecked)
        {
            return true;
        }

        if (areAllUnchecked)
        {
            return false;
        }

        return null;
    }
}