using System.Globalization;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// The typing rules behind <c>CnTimeField</c>, the sibling of
/// <see cref="CnDateMask"/>: digits are the truth, separators appear with the
/// digit behind them, and leaving the field completes what was started.
/// <para>
/// One rule is specific to a clock: a first digit above 2 cannot start a
/// two-digit hour, so it <em>is</em> the hour and the next digit already
/// belongs to the minutes. That is what makes <c>930</c> read as half past
/// nine instead of a broken 93:0.
/// </para>
/// </summary>
public static class CnTimeMask
{
    public const int MaxDigits = 6;

    /// <summary>Everything that is not a digit is noise.</summary>
    public static string Digits(string? text, bool withSeconds)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var limit = withSeconds ? MaxDigits : 4;
        Span<char> buffer = stackalloc char[MaxDigits];
        var length = 0;

        foreach (var c in text)
        {
            if (!char.IsAsciiDigit(c))
                continue;

            buffer[length++] = c;
            if (length == limit)
                break;
        }

        return new string(buffer[..length]);
    }

    /// <summary>How many digits the hour claims: one when it cannot possibly
    /// be the start of a two-digit hour.</summary>
    private static int HourLength(string digits) =>
        digits.Length > 0 && digits[0] >= '3' ? 1 : 2;

    private static (string Hour, string Minute, string Second) Split(string digits)
    {
        if (digits.Length == 0)
            return (string.Empty, string.Empty, string.Empty);

        var hourLength = HourLength(digits);
        var hour = digits[..Math.Min(hourLength, digits.Length)];
        var minute = digits.Length > hourLength
            ? digits[hourLength..Math.Min(hourLength + 2, digits.Length)]
            : string.Empty;
        var second = digits.Length > hourLength + 2
            ? digits[(hourLength + 2)..Math.Min(hourLength + 4, digits.Length)]
            : string.Empty;

        return (hour, minute, second);
    }

    /// <summary>Renders digits as <c>hh:mm</c> (or <c>hh:mm:ss</c>), growing one
    /// digit at a time.</summary>
    public static string Mask(string digits)
    {
        var (hour, minute, second) = Split(digits);
        if (hour.Length == 0)
            return string.Empty;

        var text = hour;
        if (minute.Length > 0)
            text += ":" + minute;
        if (second.Length > 0)
            text += ":" + second;

        return text;
    }

    /// <summary>Caret position just after the <paramref name="digitCount"/>-th
    /// digit — the way to keep the caret put while the text is re-rendered.</summary>
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

    /// <summary>The time the digits spell out, or <c>null</c> while they are
    /// still incomplete or spell something that does not exist.</summary>
    public static TimeOnly? Strict(string digits, bool withSeconds)
    {
        var (hour, minute, second) = Split(digits);
        if (hour.Length == 0 || minute.Length < 2)
            return null;

        if (withSeconds && second.Length < 2)
            return null;

        var h = int.Parse(hour, CultureInfo.InvariantCulture);
        var m = int.Parse(minute, CultureInfo.InvariantCulture);
        var s = second.Length == 2 ? int.Parse(second, CultureInfo.InvariantCulture) : 0;

        return h <= 23 && m <= 59 && s <= 59 ? new TimeOnly(h, m, s) : null;
    }

    /// <summary>
    /// What leaving the field makes of a half-typed time: whatever was not
    /// typed is zero, and whatever cannot exist is pulled back to the nearest
    /// value that can.
    /// <list type="bullet">
    ///   <item><description><c>9</c> → 09:00, the top of the hour.</description></item>
    ///   <item><description><c>930</c> → 09:30 — 9 cannot start a two-digit hour.</description></item>
    ///   <item><description><c>2570</c> → 23:59, the nearest time that exists.</description></item>
    /// </list>
    /// Returns <c>null</c> only when nothing at all was typed.
    /// </summary>
    public static TimeOnly? Complete(string digits)
    {
        if (digits.Length == 0)
            return null;

        var (hour, minute, second) = Split(digits);

        var h = Math.Clamp(int.Parse(hour, CultureInfo.InvariantCulture), 0, 23);
        var m = minute.Length > 0
            ? Math.Clamp(int.Parse(minute, CultureInfo.InvariantCulture), 0, 59)
            : 0;
        var s = second.Length > 0
            ? Math.Clamp(int.Parse(second, CultureInfo.InvariantCulture), 0, 59)
            : 0;

        return new TimeOnly(h, m, s);
    }

    /// <summary>Formats a time the way the mask renders it.</summary>
    public static string Format(TimeOnly? time, bool withSeconds) =>
        time is null
            ? string.Empty
            : withSeconds
                ? time.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                : time.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
}
