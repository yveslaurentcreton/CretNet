using System.Globalization;
using Fluxor;

namespace CretNet.Platform.Blazor.State.Actions;

// Actions
public record ChangeCultureAction(CultureInfo Culture);
public record ChangeCultureSuccessAction();

// Reducers
public static class ChangeCultureReducer
{
    [ReducerMethod]
    public static CnpSiteState Reduce(CnpSiteState state, ChangeCultureAction action) =>
        state with
        {
            CurrentCulture = action.Culture
        };
}
