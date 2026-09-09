using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using CretNet.Platform.Blazor.Ui.Resources;
using Microsoft.AspNetCore.Components;
using Shouldly;
using System.Globalization;

namespace CretNet.Tests.Component;

public sealed class CnIconRoleTests : BunitContext
{
    [Theory]
    [InlineData(CnActionIconStyle.Monochrome, "0 0 16 16")]
    [InlineData(CnActionIconStyle.Functional, "0 0 16 16")]
    [InlineData(CnActionIconStyle.Colored, "0 0 48 48")]
    [InlineData(CnActionIconStyle.Natural, "0 0 48 48")]
    public void Roles_EachActionStyle_LeavesEntityArtworkAndDepthIndependent(CnActionIconStyle style, string viewBox)
    {
        var actions = new CnActionIconAppearance(style);
        var cut = Render<CnIconScope>(p => p.Add(x => x.Appearance, new(CnIconStyle.Natural))
            .Add(x => x.ActionAppearance, actions).Add(x => x.ChildContent, Icons));
        var entity = cut.Find("[data-icon-role='entity']");
        entity.ClassList.ShouldContain("cn-icon--natural");
        entity.ClassList.ShouldNotContain("cn-icon--flat");
        var action = cut.Find("[data-icon-role='action']");
        action.GetAttribute("viewBox").ShouldBe(viewBox);
        if (actions.SupportsDepth) action.ClassList.ShouldContain("cn-icon--flat");
        else action.QuerySelectorAll("filter,linearGradient").ShouldBeEmpty();

        cut.Render(p => p.Add(x => x.Appearance, new(CnIconStyle.Category, CnIconIntensity.Quiet, false)));
        cut.Find("[data-icon-role='entity']").ClassList.ShouldContain("cn-icon--quiet");
        cut.Find("[data-icon-role='action']").ClassList.ShouldNotContain("cn-icon--quiet");
        cut.Find("[data-icon-role='action']").GetAttribute("viewBox").ShouldBe(viewBox);
    }

    [Fact]
    public void ActionChanges_UpdateColourAndDepth_WithoutRemountingIcons()
    {
        var cut = Render<CnIconScope>(p => p.Add(x => x.Appearance, new(CnIconStyle.Natural))
            .Add(x => x.ActionAppearance, new(CnActionIconStyle.Functional))
            .Add(x => x.ChildContent, Icons));
        cut.Find("[data-icon-role='action']").ClassList.ShouldContain("cn-icon--danger");
        cut.Render(p => p.Add(x => x.ActionAppearance, new(CnActionIconStyle.Natural, true, false)));
        var icon = cut.Find("[data-icon-role='action']");
        icon.ClassList.ShouldContain("cn-icon--natural");
        icon.ClassList.ShouldNotContain("cn-icon--flat");
        icon.InnerHtml.ShouldContain("steel-front");
        cut.Render(p => p.Add(x => x.ActionAppearance, new(CnActionIconStyle.Natural, false, true)));
        icon = cut.Find("[data-icon-role='action']");
        icon.ClassList.ShouldContain("cn-icon--flat");
        icon.InnerHtml.ShouldNotContain("steel-front");
        icon.InnerHtml.ShouldContain("#ea6975");
        cut.Render(p => p.Add(x => x.ActionAppearance, new(CnActionIconStyle.Colored, false, false)));
        cut.Find("[data-icon-role='action']").InnerHtml.ShouldContain("steel-front");
    }

    [Fact]
    public void Buttons_UseActionsForLeadingTrailingAndComposedIcons_AndKeepAccentContrast()
    {
        RenderFragment buttons = builder =>
        {
            foreach (var role in new[] { CnButtonRole.Neutral, CnButtonRole.Accent })
            {
                builder.OpenComponent<CnButton>(0);
                builder.AddAttribute(1, nameof(CnButton.Role), role);
                builder.AddAttribute(2, nameof(CnButton.Label), "Edit");
                builder.AddAttribute(3, nameof(CnButton.Icon), CnIconKind.Edit);
                builder.AddAttribute(4, nameof(CnButton.IconEnd), CnIconKind.ArrowRight);
                builder.AddAttribute(5, nameof(CnButton.ChildContent), (RenderFragment)(child =>
                {
                    child.OpenComponent<CnIcon>(0);
                    child.AddAttribute(1, nameof(CnIcon.Kind), CnIconKind.Delete);
                    child.CloseComponent();
                }));
                builder.CloseComponent();
            }
        };
        var cut = Render<CnIconScope>(p => p.Add(x => x.Appearance, new(CnIconStyle.Natural))
            .Add(x => x.ActionAppearance, new(CnActionIconStyle.Functional)).Add(x => x.ChildContent, buttons));
        cut.FindAll("button [data-icon-role='action']").Count.ShouldBe(6);
        cut.Find(".cn-btn--neutral svg").ClassList.ShouldContain("cn-icon--info");
        cut.FindAll(".cn-btn--accent svg").ShouldAllBe(icon => icon.GetAttribute("viewBox") == "0 0 16 16"
            && !icon.ClassList.Contains("cn-icon--info") && !icon.ClassList.Contains("cn-icon--danger"));
        cut.Render(p => p.Add(x => x.ActionAppearance, new(CnActionIconStyle.Natural, true)));
        cut.Find(".cn-btn--neutral svg").ClassList.ShouldContain("cn-icon--natural");
        cut.Find(".cn-btn--accent svg").GetAttribute("viewBox").ShouldBe("0 0 16 16");
    }

    [Fact]
    public void Settings_ExposeFourStyles_AndEmitIndependentActionChoices()
    {
        var entity = new CnIconAppearance(CnIconStyle.Category, CnIconIntensity.Quiet);
        var actions = new CnActionIconAppearance();
        var entityChanges = 0;
        var cut = Render<CnIconSettings>(p => p.Add(x => x.Value, entity)
            .Add(x => x.ValueChanged, _ => entityChanges++)
            .Add(x => x.ActionValue, actions).Add(x => x.ActionValueChanged, value => actions = value));
        cut.FindAll(".cn-icon-action-choices button").Count.ShouldBe(4);
        cut.FindAll("input[type='checkbox']")[1].HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".cn-icon-action-choices button:nth-child(4)").Click();
        actions.Style.ShouldBe(CnActionIconStyle.Natural);
        actions.Depth.ShouldBeTrue();
        cut.Render(p => p.Add(x => x.ActionValue, actions));
        cut.FindAll("input[type='checkbox']")[1].Change(false);
        actions.Depth.ShouldBeFalse();
        cut.Render(p => p.Add(x => x.ActionValue, actions));
        cut.FindAll("input[type='checkbox']")[2].Change(false);
        actions.DestructiveColor.ShouldBeFalse();
        entityChanges.ShouldBe(0);
        cut.Instance.Value.ShouldBe(entity);
    }

    [Fact]
    public void RoleSettings_Resources_HaveEnglishAndDutchLabels()
    {
        foreach (var key in new[] { "IconEntities", "IconActions", "IconActionStyle", "IconMonochrome",
            "IconFunctional", "IconColored", "IconNaturalMaterials", "IconDestructiveColor" })
        {
            var english = CnLabels.ResourceManager.GetString(key, CultureInfo.GetCultureInfo("en"));
            english.ShouldNotBeNullOrWhiteSpace();
            var dutch = CnLabels.ResourceManager.GetString(key, CultureInfo.GetCultureInfo("nl"));
            dutch.ShouldNotBeNullOrWhiteSpace();
            dutch.ShouldNotBe(english);
        }
    }

    private static readonly RenderFragment Icons = builder =>
    {
        builder.OpenComponent<CnIcon>(0);
        builder.AddAttribute(1, nameof(CnIcon.Kind), CnIconKind.Delete);
        builder.CloseComponent();
        builder.OpenComponent<CnIcon>(2);
        builder.AddAttribute(3, nameof(CnIcon.Kind), CnIconKind.FolderPlan);
        builder.AddAttribute(4, nameof(CnIcon.Role), CnIconRole.Entity);
        builder.CloseComponent();
    };
}
