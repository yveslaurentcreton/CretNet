using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Microsoft.AspNetCore.Components;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnButtonIconTests : BunitContext
{
    [Theory]
    [InlineData(CnIconStyle.Natural)]
    [InlineData(CnIconStyle.Category)]
    public void Accent_LeadingTrailingAndComposedIcons_UseForegroundDespiteColourPreferences(CnIconStyle style)
    {
        var clicks = 0;
        RenderFragment content = builder =>
        {
            builder.OpenComponent<CnButton>(0);
            builder.AddAttribute(1, nameof(CnButton.Role), CnButtonRole.Accent);
            builder.AddAttribute(2, nameof(CnButton.Label), "Add project");
            builder.AddAttribute(3, nameof(CnButton.Icon), CnIconKind.Add);
            builder.AddAttribute(4, nameof(CnButton.IconEnd), CnIconKind.ArrowRight);
            builder.AddAttribute(5, nameof(CnButton.ChildContent), (RenderFragment)(child =>
            {
                child.OpenComponent<CnIcon>(0);
                child.AddAttribute(1, nameof(CnIcon.Kind), CnIconKind.Save);
                child.CloseComponent();
            }));
            builder.AddAttribute(6, nameof(CnButton.OnClick), EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => clicks++));
            builder.CloseComponent();
        };
        var cut = Render<CnIconScope>(p => p
            .Add(x => x.Appearance, new(style, CnIconIntensity.Quiet, true))
            .Add(x => x.ChildContent, content));

        var icons = cut.FindAll("button svg");
        icons.Count.ShouldBe(3);
        foreach (var icon in icons)
        {
            icon.GetAttribute("viewBox").ShouldBe("0 0 16 16");
            icon.GetAttribute("aria-hidden").ShouldBe("true");
            icon.ClassList.ShouldNotContain("cn-icon--quiet");
            icon.QuerySelectorAll("linearGradient,filter").ShouldBeEmpty();
            icon.QuerySelectorAll("[stroke='currentColor']").ShouldNotBeEmpty();
        }
        cut.FindAll("button").Count.ShouldBe(1);
        cut.Find("button").GetAttribute("aria-label").ShouldBe("Add project");
        cut.Find("button").Click();
        clicks.ShouldBe(1);
    }

    [Fact]
    public void RoleAndExplicitOverride_ChangeWithoutLosingTheInheritedPreference()
    {
        RenderFragment content = builder =>
        {
            builder.OpenComponent<CnButton>(0);
            builder.AddAttribute(1, nameof(CnButton.Icon), CnIconKind.Add);
            builder.AddAttribute(2, nameof(CnButton.Title), "Add project");
            builder.CloseComponent();
        };
        var cut = Render<CnIconScope>(p => p
            .Add(x => x.Appearance, new(CnIconStyle.Natural))
            .Add(x => x.ChildContent, content));
        cut.Find("svg").ClassList.ShouldContain("cn-icon--natural");

        cut.Render(p => p.Add(x => x.Appearance, new(CnIconStyle.Category, CnIconIntensity.Quiet, false)));
        cut.Find("svg").ClassList.ShouldContain("cn-icon--category");
        cut.Find("svg").ClassList.ShouldContain("cn-icon--quiet");

        var button = cut.FindComponent<CnButton>();
        button.Render(p => p.Add(x => x.Role, CnButtonRole.Accent));
        cut.Find("svg").GetAttribute("viewBox").ShouldBe("0 0 16 16");

        button.Render(p => p.Add(x => x.IconAppearance, new CnIconAppearance(CnIconStyle.Natural)));
        cut.Find("svg").ClassList.ShouldContain("cn-icon--natural");

        button.Render(p => p.Add(x => x.IconAppearance, (CnIconAppearance?)null).Add(x => x.Role, CnButtonRole.Subtle));
        cut.Find("svg").ClassList.ShouldContain("cn-icon--category");
        cut.Find("svg").ClassList.ShouldContain("cn-icon--quiet");
        cut.Find("svg").ClassList.ShouldContain("cn-icon--flat");
    }
}
