using System.Globalization;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// The typing rules behind <c>CnDateField</c> and <c>CnDateRangeField</c>,
/// kept as pure functions so they can be reasoned about (and tested) without
/// a browser.
/// <para>
/// Digits are the only truth. Separators are drawn <em>between</em> digits
/// and only once the digit behind them exists, which is what keeps backspace
/// from having to delete a separator twice — the classic input-mask trap.
/// </para>
/// </summary>
public static class CnDateMask
{
    /// <summary>Day, month and year, in the order the mask renders them.</summary>
    private static readonly int[] SegmentWidths = [2, 2, 4];

    public const int MaxDigits = 8;

    /// <summary>Everything that is not a digit is noise: pasted text, typed
    /// separators and stray characters all reduce to the digits they carry.</summary>
    public static string Digits(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        Span<char> buffer = stackalloc char[MaxDigits];
        var length = 0;

        foreach (var c in text)
        {
            if (!char.IsAsciiDigit(c))
                continue;

            buffer[length++] = c;
            if (length == MaxDigits)
                break;
        }

        return new string(buffer[..length]);
    }

    /// <summary>Renders digits as <c>dd/mm/yyyy</c>, growing one digit at a
    /// time. A separator only appears once the segment behind it has started.</summary>
    public static string Mask(string digits, char separator = '/')
    {
        if (digits.Length == 0)
            return string.Empty;

        var text = new System.Text.StringBuilder(10);
        var index = 0;

        for (var segment = 0; segment < SegmentWidths.Length && index < digits.Length; segment++)
        {
            if (segment > 0)
                text.Append(separator);

            var take = Math.Min(SegmentWidths[segment], digits.Length - index);
            text.Append(digits.AsSpan(index, take));
            index += SegmentWidths[segment];
        }

        return text.ToString();
    }

    /// <summary>Caret position just after the <paramref name="digitCount"/>-th
    /// digit of <paramref name="masked"/> — the way to keep the caret where the
    /// user put it while the text around it is re-rendered.</summary>
    public static int CaretAfterDigit(string masked, int digitCount)
    {
        if (digitCount <= 0)
            return 0;

        var seen = 0;
        for (var i = 0; i < masked.Length; i++)
        {
            if (!char.IsAsciiDigit(masked[i]))
                continue;

            if (++seen == digitCount)
                return i + 1;
        }

        return masked.Length;
    }

    /// <summary>
    /// A day or month holding a single digit when the user types a separator
    /// is what they mean as "0x" — typing <c>1/</c> gives <c>01/</c>. Years are
    /// left alone: a partial year is not a padding candidate.
    /// </summary>
    public static string PadStartedSegment(string digits)
    {
        var index = 0;

        foreach (var width in SegmentWidths)
        {
            var filled = Math.Clamp(digits.Length - index, 0, width);
            if (filled < width)
            {
                return width == 2 && filled == 1
                    ? string.Concat(digits.AsSpan(0, index), "0", digits.AsSpan(index))
                    : digits;
            }

            index += width;
        }

        return digits;
    }

    /// <summary>The date the digits spell out, or <c>null</c> while they are
    /// still incomplete or spell something that does not exist.</summary>
    public static DateTime? Strict(string digits)
    {
        if (digits.Length < MaxDigits)
            return null;

        var day = int.Parse(digits.AsSpan(0, 2), CultureInfo.InvariantCulture);
        var month = int.Parse(digits.AsSpan(2, 2), CultureInfo.InvariantCulture);
        var year = int.Parse(digits.AsSpan(4, 4), CultureInfo.InvariantCulture);

        if (month is < 1 or > 12 || year is < 1900 or > 2999)
            return null;

        return day >= 1 && day <= DateTime.DaysInMonth(year, month)
            ? new DateTime(year, month, day)
            : null;
    }

    /// <summary>
    /// What leaving the field makes of a half-typed date: whatever was not
    /// typed comes from <paramref name="today"/>, and whatever cannot exist is
    /// pulled back to the nearest value that can.
    /// <list type="bullet">
    ///   <item><description><c>0101</c> → 1 January of this year (no year typed).</description></item>
    ///   <item><description><c>3106</c> → 30 June: June has no 31st.</description></item>
    ///   <item><description><c>3113</c> → 31 December: there is no month 13.</description></item>
    ///   <item><description><c>5</c> → the 5th of the current month.</description></item>
    ///   <item><description><c>010227</c> → 1 February 2027: a two-digit year is 20xx.</description></item>
    /// </list>
    /// Returns <c>null</c> only when nothing at all was typed.
    /// </summary>
    public static DateTime? Complete(string digits, DateTime today)
    {
        if (digits.Length == 0)
            return null;

        var dayText = digits.Length >= 2 ? digits[..2] : digits;
        var monthText = digits.Length > 2 ? digits[2..Math.Min(4, digits.Length)] : string.Empty;
        var yearText = digits.Length > 4 ? digits[4..] : string.Empty;

        var month = monthText.Length > 0
            ? Math.Clamp(int.Parse(monthText, CultureInfo.InvariantCulture), 1, 12)
            : today.Month;

        var year = yearText.Length switch
        {
            4 => Math.Clamp(int.Parse(yearText, CultureInfo.InvariantCulture), 1900, 2999),
            2 => 2000 + int.Parse(yearText, CultureInfo.InvariantCulture),
            _ => today.Year,
        };

        var day = int.Parse(dayText, CultureInfo.InvariantCulture);
        day = Math.Clamp(day == 0 ? 1 : day, 1, DateTime.DaysInMonth(year, month));

        return new DateTime(year, month, day);
    }

    /// <summary>Formats a date the way the mask renders it.</summary>
    public static string Format(DateTime? date, char separator = '/') =>
        date is null
            ? string.Empty
            : Mask($"{date.Value:ddMMyyyy}", separator);
}
