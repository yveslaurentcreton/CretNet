using Fluxor;

namespace CretNet.Platform.Blazor.State;

public class CnpSiteFeature : Feature<CnpSiteState>
{
    public override string GetName() => nameof(CnpSiteState);

    protected override CnpSiteState GetInitialState() =>
        new CnpSiteState(
            CurrentCulture: null);
}