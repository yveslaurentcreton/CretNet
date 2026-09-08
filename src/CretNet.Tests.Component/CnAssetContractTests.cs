using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnAssetContractTests : BunitContext
{
    [Fact]
    public void Theme_ProvidesNotificationTokenAliases()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cn-ui.css"));
        css.ShouldContain("--cn-surface: var(--cn-card);");
        css.ShouldContain("--cn-surface-muted: var(--cn-ground);");
        css.ShouldContain("--cn-bg: var(--cn-ground);");
        css.ShouldContain("--cn-border: var(--cn-stroke);");
        css.ShouldContain("--cn-text-muted: var(--cn-text-2);");
        css.ShouldContain("--cn-accent-strong: var(--cn-accent-hover);");
    }

    [Fact]
    public void Shell_BrandImageFillsItsBoundedMark()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cn-shell.css"));
        css.ShouldContain(".cn-brand-mark img { width: 100%; height: 100%; display: block; }");
        css.ShouldContain("overflow: hidden;");
    }

    [Fact]
    public void SettingsIcon_UsesUniformlyScaledStandardGeometry()
    {
        var icon = Render<CnIcon>(p => p.Add(x => x.Kind, CnIconKind.Settings));
        icon.Find("g").GetAttribute("transform").ShouldBe("scale(.666667)");
        var circle = icon.Find("circle");
        circle.GetAttribute("cx").ShouldBe("12");
        circle.GetAttribute("cy").ShouldBe("12");
        circle.GetAttribute("r").ShouldBe("3");
        icon.Markup.ShouldContain("stroke-linecap=\"round\"");
        icon.Markup.ShouldNotContain("M6.7 1.75");
    }
}
