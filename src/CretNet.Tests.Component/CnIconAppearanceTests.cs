using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using CretNet.Platform.Blazor.Ui.Resources;
using Microsoft.AspNetCore.Components;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnIconAppearanceTests : BunitContext
{
    public static IEnumerable<object[]> ArtworkCases =>
        from kind in Enum.GetValues<CnIconKind>()
        from style in new[] { CnIconStyle.Natural, CnIconStyle.Category }
        select new object[] { kind, style };

    [Theory]
    [MemberData(nameof(ArtworkCases))]
    public void Artwork_EachKindAndStyle_HasValidSelfContainedSvg(CnIconKind kind, CnIconStyle style)
    {
        var cut = Render<CnIcon>(p => p.Add(x => x.Kind, kind).Add(x => x.Appearance, new(style)));
        cut.Find("svg").GetAttribute("viewBox").ShouldBe("0 0 48 48");
        cut.Find("svg").GetAttribute("aria-hidden").ShouldBe("true");
        cut.FindAll("path,rect,circle,polygon,ellipse").ShouldNotBeEmpty();
        cut.Markup.ShouldNotContain("__CN_ICON__");
        var xml = XDocument.Parse(cut.Markup);
        var ids = xml.Descendants().Attributes("id").Select(attribute => attribute.Value).ToArray();
        ids.Distinct().Count().ShouldBe(ids.Length);
        foreach (Match reference in Regex.Matches(cut.Markup, @"url\(#([^\)]+)\)"))
            ids.ShouldContain(reference.Groups[1].Value);
    }

    [Fact]
    public void Scope_ChangesAppearance_UpdatesDescendantsWithoutChangingExplicitOverrides()
    {
        RenderFragment content = builder =>
        {
            builder.OpenComponent<CnIcon>(0);
            builder.AddAttribute(1, nameof(CnIcon.Kind), CnIconKind.DocumentSpark);
            builder.CloseComponent();
            builder.OpenComponent<CnIcon>(2);
            builder.AddAttribute(3, nameof(CnIcon.Kind), CnIconKind.DocumentSpark);
            builder.CloseComponent();
            builder.OpenComponent<CnIcon>(4);
            builder.AddAttribute(5, nameof(CnIcon.Kind), CnIconKind.Settings);
            builder.AddAttribute(6, nameof(CnIcon.Appearance), new CnIconAppearance());
            builder.CloseComponent();
        };
        var cut = Render<CnIconScope>(p => p.Add(x => x.Appearance, new(CnIconStyle.Natural))
            .Add(x => x.ChildContent, content));
        var ids = cut.FindAll("[id]").Select(x => x.Id).ToArray();
        ids.Distinct().Count().ShouldBe(ids.Length);
        cut.FindAll("svg.cn-icon--natural").Count.ShouldBe(2);
        cut.Render(p => p.Add(x => x.Appearance,
            new(CnIconStyle.Category, CnIconIntensity.Quiet, false, CnNavigationIconSize.Large)));
        cut.FindAll("svg.cn-icon--category.cn-icon--quiet.cn-icon--flat").Count.ShouldBe(2);
        cut.FindAll("svg")[2].GetAttribute("viewBox").ShouldBe("0 0 16 16");
        cut.Find(".cn-icon-scope").GetAttribute("style")!.ShouldContain("26px");
    }

    [Fact]
    public void Settings_AllControls_EmitIndependentPreferences()
    {
        var value = new CnIconAppearance(CnIconStyle.Natural);
        var cut = Render<CnIconSettings>(p => p.Add(x => x.Value, value)
            .Add(x => x.ValueChanged, changed => value = changed));
        cut.Find("[aria-label='Icon colours'] button:nth-child(2)").Click();
        value.Style.ShouldBe(CnIconStyle.Category);
        cut.Render(p => p.Add(x => x.Value, value));
        cut.Find("[aria-label='Colour intensity'] button:nth-child(2)").Click();
        value.Intensity.ShouldBe(CnIconIntensity.Quiet);
        cut.Render(p => p.Add(x => x.Value, value));
        cut.Find("[aria-label='Sidebar icon size'] button:nth-child(3)").Click();
        value.NavigationSize.ShouldBe(CnNavigationIconSize.Large);
        cut.Render(p => p.Add(x => x.Value, value));
        cut.Find("input[type=checkbox]").Change(false);
        value.Depth.ShouldBeFalse();
        value.Style.ShouldBe(CnIconStyle.Category);
        value.Intensity.ShouldBe(CnIconIntensity.Quiet);
    }

    [Fact]
    public void Settings_Resources_HaveEnglishAndDutchWording()
    {
        foreach (var key in new[] { "IconColorStyle", "IconNaturalColors", "IconCategoryColors", "IconColorIntensity",
            "IconNormalIntensity", "IconQuietIntensity", "IconNavigationSize", "IconSubtleDepth" })
        {
            CnLabels.ResourceManager.GetString(key, CultureInfo.GetCultureInfo("en")).ShouldNotBeNullOrEmpty();
            CnLabels.ResourceManager.GetString(key, CultureInfo.GetCultureInfo("nl"))
                .ShouldNotBe(CnLabels.ResourceManager.GetString(key, CultureInfo.GetCultureInfo("en")));
        }
    }
}
