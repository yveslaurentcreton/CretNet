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
        from style in Enum.GetValues<CnIconStyle>()
        select new object[] { kind, style };

    [Theory]
    [MemberData(nameof(ArtworkCases))]
    public void Artwork_EachKindAndStyle_HasValidSelfContainedSvg(CnIconKind kind, CnIconStyle style)
    {
        var cut = Render<CnIcon>(p => p.Add(x => x.Kind, kind).Add(x => x.Appearance, new(style)));
        cut.Find("svg").GetAttribute("viewBox").ShouldBe(style.Family() switch
        {
            CnIconFamily.Outline => "0 0 16 16",
            CnIconFamily.Natural => "0 0 48 48",
            _ => "0 0 24 24",
        });
        cut.Find("svg").GetAttribute("aria-hidden").ShouldBe("true");
        cut.FindAll("path,rect,circle,polygon,ellipse").ShouldNotBeEmpty();
        cut.Markup.ShouldNotContain("__CN_ICON__");
        cut.Markup.ShouldNotContain("data-solid");
        var xml = XDocument.Parse(cut.Markup);
        var ids = xml.Descendants().Attributes("id").Select(attribute => attribute.Value).ToArray();
        ids.Distinct().Count().ShouldBe(ids.Length);
        ids.ShouldAllBe(id => id.StartsWith("cn-icon-"));
        // Every drawn icon has its one coloured accent.
        if (style.Family() == CnIconFamily.Porcelain)
            cut.FindAll("[stroke-width='1.8']").ShouldAllBe(accent => Regex.IsMatch(accent.GetAttribute("stroke")!, "^#[0-9a-f]{6}$"));
        if (style.Family() == CnIconFamily.Porcelain)
            cut.FindAll("[stroke-width='1.8']").ShouldNotBeEmpty();
        foreach (Match reference in Regex.Matches(cut.Markup, @"url\(#([^\)]+)\)"))
            ids.ShouldContain(reference.Groups[1].Value);
    }

    [Fact]
    public void Porcelain_AccentFollowsTheColourMode_WithATintedOutlineAndGreyDetails()
    {
        var own = Render<CnIcon>(p => p.Add(x => x.Kind, CnIconKind.People).Add(x => x.Appearance, new(CnIconStyle.Porcelain)));
        own.Find("svg").ClassList.ShouldContain("cn-icon--porcelain");
        own.Find("svg").ClassList.ShouldNotContain("cn-icon--by-category");
        // Body: porcelain gradient with an outline of 45% accent (#458fdc) and 55% grey.
        own.FindAll("[stroke='#729bc5'][stroke-width='1.05']").Count.ShouldBe(2);
        own.Find("linearGradient stop.cn-icon-tone-lo").GetAttribute("stop-color").ShouldBe("#dfe5eb");
        own.FindAll("[stroke='#458fdc'][stroke-width='1.8']").Count.ShouldBe(2);
        // Closed accent shapes get a 22% tint, open ones stay unfilled.
        own.Find("circle[stroke='#458fdc']").GetAttribute("fill").ShouldBe("#d6e6f7");
        own.Find("path[stroke='#458fdc']").GetAttribute("fill").ShouldBe("none");
        own.Find("g.cn-icon-shadow").GetAttribute("filter").ShouldStartWith("url(#cn-icon-");

        var calculator = Render<CnIcon>(p => p.Add(x => x.Kind, CnIconKind.Calculator)
            .Add(x => x.Appearance, new(CnIconStyle.PorcelainCategory, CnIconIntensity.Quiet, false)));
        calculator.Find("svg").ClassList.ShouldContain("cn-icon--by-category");
        calculator.Find("svg").ClassList.ShouldContain("cn-icon--quiet");
        calculator.Find("svg").ClassList.ShouldContain("cn-icon--flat");
        calculator.FindAll("[fill='#c3ced8']").Count.ShouldBe(5);
        calculator.FindAll("[stroke='#39ad64'][stroke-width='1.8']").Count.ShouldBe(1);
        // The data-solid key is filled solid in the accent colour.
        calculator.FindAll("rect[fill='#39ad64']:not([stroke])").Count.ShouldBe(1);
    }

    [Theory]
    [InlineData(CnIconKind.Calculator, "#4da96d")]
    [InlineData(CnIconKind.Checkmark, "#4da96d")]
    [InlineData(CnIconKind.Delete, "#ea6975")]
    [InlineData(CnIconKind.Search, "#388ce7")]
    [InlineData(CnIconKind.Document, "#388ce7")]
    [InlineData(CnIconKind.Settings, "#7c91ac")]
    public void Porcelain_OwnAccent_IsTheNaturalColourOrTheCategoryFallback(CnIconKind kind, string accent)
    {
        var cut = Render<CnIcon>(p => p.Add(x => x.Kind, kind).Add(x => x.Appearance, new(CnIconStyle.Porcelain)));
        cut.FindAll("[stroke-width='1.8']").ShouldAllBe(element => element.GetAttribute("stroke") == accent);
    }

    [Fact]
    public void Porcelain_OwnAccent_IsNeverASharedNeutralMaterial()
    {
        // Glass, paper, steel and graphite are shared by the artwork, never an icon's own colour.
        string[] neutral = ["#c5e5f0", "#f4f6f8", "#9daebb", "#48566a", "#e7edf3"];
        foreach (var kind in Enum.GetValues<CnIconKind>())
        {
            var cut = Render<CnIcon>(p => p.Add(x => x.Kind, kind).Add(x => x.Appearance, new(CnIconStyle.Porcelain)));
            foreach (var element in cut.FindAll("[stroke-width='1.8']"))
                neutral.ShouldNotContain(element.GetAttribute("stroke"), kind.ToString());
        }
    }

    [Fact]
    public void Line_OwnColourFollowsTheText_CategoryUsesALighterCategoryColour()
    {
        var own = Render<CnIcon>(p => p.Add(x => x.Kind, CnIconKind.People).Add(x => x.Appearance, new(CnIconStyle.Line)));
        own.Find("svg").ClassList.ShouldContain("cn-icon--line");
        var strokes = own.Find("svg > g");
        strokes.GetAttribute("stroke").ShouldBe("currentColor");
        strokes.GetAttribute("stroke-width").ShouldBe("1.3");
        strokes.GetAttribute("fill").ShouldBe("none");
        strokes.GetAttribute("stroke-linecap").ShouldBe("round");
        strokes.GetAttribute("stroke-linejoin").ShouldBe("round");
        strokes.Children.Length.ShouldBe(4);
        own.FindAll("[id]").ShouldBeEmpty();

        var category = Render<CnIcon>(p => p.Add(x => x.Kind, CnIconKind.People).Add(x => x.Appearance, new(CnIconStyle.LineCategory)));
        category.Find("svg").ClassList.ShouldContain("cn-icon--by-category");
        // #388ce7 lightened 18% toward white.
        category.Find("svg > g").GetAttribute("stroke").ShouldBe("#5ca1eb");
    }

    [Fact]
    public void Styles_PersistedNamesAndNumbers_KeepTheirMeaning()
    {
        ((int)CnIconStyle.Outline).ShouldBe(0);
        ((int)CnIconStyle.Natural).ShouldBe(1);
        ((int)CnIconStyle.Category).ShouldBe(2);
        Enum.Parse<CnIconStyle>("Natural").ShouldBe(CnIconStyle.Natural);
        Enum.Parse<CnIconStyle>("Category").ShouldBe(CnIconStyle.Category);
        ((int)CnIconKind.Link).ShouldBe(67);
        ((int)CnIconKind.Assistant).ShouldBe(68);
        ((int)CnIconKind.History).ShouldBe(69);
        ((int)CnIconKind.Compose).ShouldBe(70);
        ((int)CnIconKind.Shield).ShouldBe(71);
        ((int)CnIconKind.ChevronDown).ShouldBe(72);
        ((int)CnIconKind.LayoutNarrow).ShouldBe(73);
        ((int)CnIconKind.LayoutHalf).ShouldBe(74);
        ((int)CnIconKind.LayoutFull).ShouldBe(75);
        ((int)CnIconKind.LayoutWindow).ShouldBe(76);
        foreach (var style in Enum.GetValues<CnIconStyle>())
            CnIconStyleExtensions.Compose(style.Family(), style.ColorMode()).ShouldBe(
                style == CnIconStyle.Outline ? CnIconStyle.Outline : style);
        CnIconStyle.Category.Family().ShouldBe(CnIconFamily.Natural);
        CnIconStyle.Category.ColorMode().ShouldBe(CnIconColorMode.Category);
        CnIconStyle.Natural.WithFamily(CnIconFamily.Porcelain).ShouldBe(CnIconStyle.Porcelain);
        CnIconStyle.Category.WithFamily(CnIconFamily.Line).ShouldBe(CnIconStyle.LineCategory);
        CnIconStyle.LineCategory.WithColorMode(CnIconColorMode.Own).ShouldBe(CnIconStyle.Line);
        CnIconStyle.Line.SupportsDepth().ShouldBeFalse();
        CnIconStyle.PorcelainCategory.SupportsDepth().ShouldBeTrue();
    }

    public static IEnumerable<object[]> AssistantPanelKinds =>
        from kind in new[]
        {
            CnIconKind.History, CnIconKind.Compose, CnIconKind.Shield, CnIconKind.ChevronDown,
            CnIconKind.LayoutNarrow, CnIconKind.LayoutHalf, CnIconKind.LayoutFull, CnIconKind.LayoutWindow,
            CnIconKind.QuestionCircle, CnIconKind.SignOut, CnIconKind.BookOpen,
        }
        select new object[] { kind };

    [Theory]
    [MemberData(nameof(AssistantPanelKinds))]
    public void AssistantPanelKinds_DrawDistinctArtworkInEveryStyle(CnIconKind kind)
    {
        foreach (var style in Enum.GetValues<CnIconStyle>())
        {
            var markup = Normalized(kind, style);
            markup.ShouldNotBeNullOrWhiteSpace();
            // None repeats another kind's drawing in the same style.
            foreach (var other in Enum.GetValues<CnIconKind>().Where(other => other != kind))
                Normalized(other, style).ShouldNotBe(markup, $"{kind} and {other} in {style}");
        }
    }

    [Theory]
    [InlineData(CnIconKind.LayoutNarrow)]
    [InlineData(CnIconKind.LayoutHalf)]
    [InlineData(CnIconKind.LayoutFull)]
    [InlineData(CnIconKind.LayoutWindow)]
    public void LayoutKinds_TheAssistantsPart_IsTheAssistantsViolet(CnIconKind kind)
    {
        var own = Render<CnIcon>(p => p.Add(x => x.Kind, kind).Add(x => x.Appearance, new(CnIconStyle.Porcelain)));
        own.FindAll("[stroke-width='1.8']").ShouldAllBe(accent => accent.GetAttribute("stroke") == "#8a63d2");
        var category = Render<CnIcon>(p => p.Add(x => x.Kind, kind).Add(x => x.Appearance, new(CnIconStyle.PorcelainCategory)));
        category.FindAll("[stroke-width='1.8']").ShouldAllBe(accent => accent.GetAttribute("stroke") == "#9162dc");
        // The outline shades the assistant's part.
        var outline = Render<CnIcon>(p => p.Add(x => x.Kind, kind));
        outline.FindAll("[fill-opacity='.35']").ShouldNotBeEmpty();
    }

    private string Normalized(CnIconKind kind, CnIconStyle style) => Regex.Replace(
        Render<CnIcon>(p => p.Add(x => x.Kind, kind).Add(x => x.Appearance, new(style))).Find("svg").InnerHtml,
        "cn-icon-[0-9]+", "cn-icon");

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
        cut.Find("[aria-label='Colour'] button:nth-child(2)").Click();
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
    public void Settings_StyleAndColour_AreIndependentAxes()
    {
        var value = new CnIconAppearance(CnIconStyle.Category);
        var cut = Render<CnIconSettings>(p => p.Add(x => x.Value, value)
            .Add(x => x.ValueChanged, changed => value = changed));
        cut.FindAll("[aria-label='Style'] button").Select(x => x.TextContent.Trim())
            .ShouldBe(["Natural", "Porcelain", "Line"]);
        cut.FindAll("[aria-label='Style'] button svg").Count.ShouldBe(3);
        cut.Find("[aria-label='Style'] button:nth-child(2)").Click();
        value.Style.ShouldBe(CnIconStyle.PorcelainCategory);
        cut.Render(p => p.Add(x => x.Value, value));
        cut.Find("[aria-label='Colour'] button:nth-child(1)").Click();
        value.Style.ShouldBe(CnIconStyle.Porcelain);
        cut.Render(p => p.Add(x => x.Value, value));
        cut.Find("input[type=checkbox]").HasAttribute("disabled").ShouldBeFalse();
        cut.Find("[aria-label='Style'] button:nth-child(3)").Click();
        value.Style.ShouldBe(CnIconStyle.Line);
        cut.Render(p => p.Add(x => x.Value, value));
        cut.Find("[aria-label='Style'] button:nth-child(3)").GetAttribute("aria-pressed").ShouldBe("true");
        cut.Find("[aria-label='Colour'] button:nth-child(1)").GetAttribute("aria-pressed").ShouldBe("true");
        // Depth has no meaning for thin strokes.
        cut.Find("input[type=checkbox]").HasAttribute("disabled").ShouldBeTrue();
        cut.Find("[aria-label='Colour'] button:nth-child(2) svg").ClassList.ShouldContain("cn-icon--by-category");
    }

    [Fact]
    public void Settings_Resources_HaveEnglishAndDutchWording()
    {
        foreach (var key in new[] { "IconColorStyle", "IconNaturalColors", "IconCategoryColors", "IconColorIntensity",
            "IconNormalIntensity", "IconQuietIntensity", "IconNavigationSize", "IconSubtleDepth",
            "IconStyle", "IconColorMode", "IconOwnColors", "IconPorcelain", "IconLine" })
        {
            CnLabels.ResourceManager.GetString(key, CultureInfo.GetCultureInfo("en")).ShouldNotBeNullOrEmpty();
            CnLabels.ResourceManager.GetString(key, CultureInfo.GetCultureInfo("nl"))
                .ShouldNotBe(CnLabels.ResourceManager.GetString(key, CultureInfo.GetCultureInfo("en")));
        }
    }
}
