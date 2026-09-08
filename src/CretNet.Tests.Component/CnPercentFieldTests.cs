using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

public class CnPercentFieldTests : CnTestContext
{
    [Fact]
    public void Percentage_UsesTextInputWithoutSpinners_AndAcceptsZero()
    {
        decimal? value = null;
        var cut = Render<CnPercentField>(parameters => parameters
            .Add(component => component.Value, 21m)
            .Add(component => component.ValueChanged, changed => value = changed));

        var input = cut.Find("input");
        input.GetAttribute("type").ShouldBe("text");
        input.GetAttribute("inputmode").ShouldBe("decimal");
        input.Change("0");

        value.ShouldBe(0m);
    }

    [Fact]
    public void Percentage_AcceptsBelgianDecimalComma()
    {
        decimal? value = null;
        var cut = Render<CnPercentField>(parameters => parameters
            .Add(component => component.ValueChanged, changed => value = changed));

        cut.Find("input").Change("6,5 %");

        value.ShouldBe(6.5m);
    }
}
