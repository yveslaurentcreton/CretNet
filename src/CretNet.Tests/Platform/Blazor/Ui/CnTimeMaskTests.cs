using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Platform.Blazor.Ui;

/// <summary>
/// The typing contract of <c>CnTimeField</c>. The rule that earns its keep is
/// the short hour: a first digit above 2 cannot begin a two-digit hour, so it
/// is the hour and typing carries straight on into the minutes.
/// </summary>
public class CnTimeMaskTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("1", "1")]
    [InlineData("13", "13")]
    [InlineData("134", "13:4")]
    [InlineData("1345", "13:45")]
    [InlineData("9", "9")]
    [InlineData("93", "9:3")]        // 9 cannot start a two-digit hour
    [InlineData("930", "9:30")]
    public void Mask_grows_one_digit_at_a_time(string digits, string expected) =>
        CnTimeMask.Mask(digits).ShouldBe(expected);

    [Fact]
    public void Mask_carries_seconds_when_they_are_typed()
    {
        CnTimeMask.Mask("134523").ShouldBe("13:45:23");
        CnTimeMask.Mask("93023").ShouldBe("9:30:23");
    }

    [Fact]
    public void Digits_stops_at_the_precision_the_field_asks_for()
    {
        CnTimeMask.Digits("13:45:23", withSeconds: false).ShouldBe("1345");
        CnTimeMask.Digits("13:45:23", withSeconds: true).ShouldBe("134523");
        CnTimeMask.Digits(null, withSeconds: true).ShouldBe(string.Empty);
    }

    [Fact]
    public void Strict_only_accepts_a_complete_time_that_exists()
    {
        CnTimeMask.Strict("1345", withSeconds: false).ShouldBe(new TimeOnly(13, 45));
        CnTimeMask.Strict("930", withSeconds: false).ShouldBe(new TimeOnly(9, 30));   // 9 cannot start a two-digit hour
        CnTimeMask.Strict("93", withSeconds: false).ShouldBeNull();       // still typing the minutes
        CnTimeMask.Strict("2570", withSeconds: false).ShouldBeNull();      // 25:70 exists nowhere
        CnTimeMask.Strict("1345", withSeconds: true).ShouldBeNull();       // seconds still missing
        CnTimeMask.Strict("134523", withSeconds: true).ShouldBe(new TimeOnly(13, 45, 23));
    }

    [Theory]
    [InlineData("9", 9, 0, 0)]         // an hour on its own → the top of it
    [InlineData("930", 9, 30, 0)]      // the short-hour rule
    [InlineData("1345", 13, 45, 0)]
    [InlineData("7", 7, 0, 0)]         // morning: the 24-hour clock stays leading
    [InlineData("2570", 23, 59, 0)]    // pulled back to the nearest real time
    [InlineData("045", 4, 5, 0)]
    [InlineData("134523", 13, 45, 23)]
    [InlineData("93099", 9, 30, 59)]   // seconds clamp too
    public void Complete_fills_in_what_was_not_typed_and_pulls_back_what_cannot_exist(
        string digits, int hour, int minute, int second) =>
        CnTimeMask.Complete(digits).ShouldBe(new TimeOnly(hour, minute, second));

    [Fact]
    public void Complete_returns_null_for_an_empty_field() =>
        CnTimeMask.Complete(string.Empty).ShouldBeNull();

    [Fact]
    public void CaretAfterDigit_lands_behind_the_nth_digit()
    {
        CnTimeMask.CaretAfterDigit("13:45", 2).ShouldBe(2);
        CnTimeMask.CaretAfterDigit("13:45", 3).ShouldBe(4);
        CnTimeMask.CaretAfterDigit("13:45", 0).ShouldBe(0);
    }

    [Fact]
    public void Format_round_trips_through_the_mask()
    {
        CnTimeMask.Format(new TimeOnly(9, 5), withSeconds: false).ShouldBe("09:05");
        CnTimeMask.Format(new TimeOnly(9, 5, 3), withSeconds: true).ShouldBe("09:05:03");
        CnTimeMask.Format(null, withSeconds: true).ShouldBe(string.Empty);
    }
}
