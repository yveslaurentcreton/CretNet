namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>Lifecycle state of one <see cref="CnFlowRail"/> step (S-093/M-018 flow-rail per REQ-004 #7).</summary>
public enum CnFlowState
{
    Pending,
    Current,
    Done,
}

/// <summary>One step of a <see cref="CnFlowRail"/> — mirrors the approved mockup's
/// renderFlowRail(steps): a short label, a detail line (amount, progress, count...), a
/// lifecycle state, and an optional click-through to the connected document/section.</summary>
public sealed record CnFlowStep(string Label, string? Detail, CnFlowState State, Action? OnClick = null);
