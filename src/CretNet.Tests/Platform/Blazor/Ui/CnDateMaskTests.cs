using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Platform.Blazor.Ui;

/// <summary>
/// The typing contract of the Cn date fields. These rules are what make the
/// control quick to type into, so they are pinned here rather than left to
/// the browser to prove.
/// </summary>
public class CnDateMaskTests
{
    private static readonly DateTime Today = new(2026, 8, 26);

    [Theory]
    [InlineData("", "")]
    [InlineData("1", "1")]
    [InlineData("10", "10")]
    [InlineData("100", "10/0")]
    [InlineData("1008", "10/08")]
    [InlineData("10082", "10/08/2")]
    [InlineData("10082026", "10/08/2026")]
    public void Mask_draws_separators_only_once_the_digit_behind_them_exists(string digits, string expected) =>
        CnDateMask.Mask(digits).ShouldBe(expected);

    [Fact]
    public void Digits_keeps_only_digits_and_stops_at_a_full_date()
    {
        CnDateMask.Digits("10/08/2026").ShouldBe("10082026");
        CnDateMask.Digits("1/8/26").ShouldBe("1826");
        CnDateMask.Digits("10/08/2026 and more").ShouldBe("10082026");
        CnDateMask.Digits(null).ShouldBe(string.Empty);
    }

    [Fact]
    public void CaretAfterDigit_lands_behind_the_nth_digit()
    {
        // "10/08/2026": the 2nd digit ends at index 2, the 3rd sits past the slash.
        CnDateMask.CaretAfterDigit("10/08/2026", 2).ShouldBe(2);
        CnDateMask.CaretAfterDigit("10/08/2026", 3).ShouldBe(4);
        CnDateMask.CaretAfterDigit("10/08/2026", 0).ShouldBe(0);
        CnDateMask.CaretAfterDigit("10/08/2026", 99).ShouldBe(10);
    }

    [Theory]
    [InlineData("1", "01")]          // typing "1/" means the first
    [InlineData("108", "1008")]      // typing "10/8/" means August
    [InlineData("10", "10")]         // a full segment needs no padding
    [InlineData("100820", "100820")] // a partial year is never padded
    public void PadStartedSegment_treats_a_typed_separator_as_a_leading_zero(string digits, string expected) =>
        CnDateMask.PadStartedSegment(digits).ShouldBe(expected);

    [Fact]
    public void Strict_only_accepts_a_complete_date_that_exists()
    {
        CnDateMask.Strict("10082026").ShouldBe(new DateTime(2026, 8, 10));
        CnDateMask.Strict("1008202").ShouldBeNull();      // still typing
        CnDateMask.Strict("31062026").ShouldBeNull();     // June has 30 days
        CnDateMask.Strict("32082026").ShouldBeNull();
        CnDateMask.Strict("10132026").ShouldBeNull();     // no month 13
        CnDateMask.Strict("29022026").ShouldBeNull();     // 2026 is not a leap year
        CnDateMask.Strict("29022028").ShouldBe(new DateTime(2028, 2, 29));
    }

    [Theory]
    [InlineData("0101", 2026, 1, 1)]    // no year typed → this year
    [InlineData("3101", 2026, 1, 31)]   // January really has 31 days
    [InlineData("3106", 2026, 6, 30)]   // June has 30 → pulled back
    [InlineData("3102", 2026, 2, 28)]   // February 2026 has 28
    [InlineData("3113", 2026, 12, 31)]  // month 13 → December
    [InlineData("5", 2026, 8, 5)]       // day only → this month
    [InlineData("15", 2026, 8, 15)]
    [InlineData("010227", 2027, 2, 1)]  // two-digit year → 20xx
    [InlineData("15082026", 2026, 8, 15)]
    public void Complete_fills_in_what_was_not_typed_and_pulls_back_what_cannot_exist(
        string digits, int year, int month, int day) =>
        CnDateMask.Complete(digits, Today).ShouldBe(new DateTime(year, month, day));

    [Fact]
    public void Complete_returns_null_for_an_empty_field()
    {
        CnDateMask.Complete(string.Empty, Today).ShouldBeNull();
    }

    [Fact]
    public void Complete_keeps_a_leap_day_that_the_typed_year_allows()
    {
        // 29/02 with an explicit leap year survives; without a year it falls
        // back to this year, where February is one day shorter.
        CnDateMask.Complete("29022028", Today).ShouldBe(new DateTime(2028, 2, 29));
        CnDateMask.Complete("2902", Today).ShouldBe(new DateTime(2026, 2, 28));
    }

    [Fact]
    public void Format_round_trips_through_the_mask()
    {
        CnDateMask.Format(new DateTime(2026, 8, 1)).ShouldBe("01/08/2026");
        CnDateMask.Format(null).ShouldBe(string.Empty);
    }
}
