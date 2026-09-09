using System.Globalization;
using System.Resources;

namespace CretNet.Platform.Blazor.Ui.Resources;

/// <summary>Control and generic UI wording owned by CretNet. English fallback,
/// Dutch satellite resources; hosts own their application-specific resources.</summary>
public static class CnLabels
{
    public static ResourceManager ResourceManager { get; } = new(
        "CretNet.Platform.Blazor.Ui.Resources.CnLabels", typeof(CnLabels).Assembly);

    public static string Format(string format, params object[] arguments) =>
        string.Format(CultureInfo.CurrentCulture, format, arguments);

    public static string Action => ResourceManager.GetString(nameof(Action), CultureInfo.CurrentUICulture)!;
    public static string ActionNeeded => ResourceManager.GetString(nameof(ActionNeeded), CultureInfo.CurrentUICulture)!;
    public static string AdvancedSearch => ResourceManager.GetString(nameof(AdvancedSearch), CultureInfo.CurrentUICulture)!;
    public static string All => ResourceManager.GetString(nameof(All), CultureInfo.CurrentUICulture)!;
    public static string Amount => ResourceManager.GetString(nameof(Amount), CultureInfo.CurrentUICulture)!;
    public static string Archive => ResourceManager.GetString(nameof(Archive), CultureInfo.CurrentUICulture)!;
    public static string Bold => ResourceManager.GetString(nameof(Bold), CultureInfo.CurrentUICulture)!;
    public static string Breadcrumb => ResourceManager.GetString(nameof(Breadcrumb), CultureInfo.CurrentUICulture)!;
    public static string BulletList => ResourceManager.GetString(nameof(BulletList), CultureInfo.CurrentUICulture)!;
    public static string Cancel => ResourceManager.GetString(nameof(Cancel), CultureInfo.CurrentUICulture)!;
    public static string ChangeStatus => ResourceManager.GetString(nameof(ChangeStatus), CultureInfo.CurrentUICulture)!;
    public static string Clear => ResourceManager.GetString(nameof(Clear), CultureInfo.CurrentUICulture)!;
    public static string ClearFormatting => ResourceManager.GetString(nameof(ClearFormatting), CultureInfo.CurrentUICulture)!;
    public static string Close => ResourceManager.GetString(nameof(Close), CultureInfo.CurrentUICulture)!;
    public static string CouldNotLoadNotifications => ResourceManager.GetString(nameof(CouldNotLoadNotifications), CultureInfo.CurrentUICulture)!;
    public static string CtrlEnterToPost => ResourceManager.GetString(nameof(CtrlEnterToPost), CultureInfo.CurrentUICulture)!;
    public static string Date => ResourceManager.GetString(nameof(Date), CultureInfo.CurrentUICulture)!;
    public static string DateFormatHint => ResourceManager.GetString(nameof(DateFormatHint), CultureInfo.CurrentUICulture)!;
    public static string Day => ResourceManager.GetString(nameof(Day), CultureInfo.CurrentUICulture)!;
    public static string Days => ResourceManager.GetString(nameof(Days), CultureInfo.CurrentUICulture)!;
    public static string DaysAgo => ResourceManager.GetString(nameof(DaysAgo), CultureInfo.CurrentUICulture)!;
    public static string Dismiss => ResourceManager.GetString(nameof(Dismiss), CultureInfo.CurrentUICulture)!;
    public static string Earlier => ResourceManager.GetString(nameof(Earlier), CultureInfo.CurrentUICulture)!;
    public static string Ellipsis => ResourceManager.GetString(nameof(Ellipsis), CultureInfo.CurrentUICulture)!;
    public static string EmptyValue => ResourceManager.GetString(nameof(EmptyValue), CultureInfo.CurrentUICulture)!;
    public static string From => ResourceManager.GetString(nameof(From), CultureInfo.CurrentUICulture)!;
    public static string HoursAgo => ResourceManager.GetString(nameof(HoursAgo), CultureInfo.CurrentUICulture)!;
    public static string Italic => ResourceManager.GetString(nameof(Italic), CultureInfo.CurrentUICulture)!;
    public static string Item => ResourceManager.GetString(nameof(Item), CultureInfo.CurrentUICulture)!;
    public static string JustNow => ResourceManager.GetString(nameof(JustNow), CultureInfo.CurrentUICulture)!;
    public static string Loading => ResourceManager.GetString(nameof(Loading), CultureInfo.CurrentUICulture)!;
    public static string MarkAllRead => ResourceManager.GetString(nameof(MarkAllRead), CultureInfo.CurrentUICulture)!;
    public static string MarkRead => ResourceManager.GetString(nameof(MarkRead), CultureInfo.CurrentUICulture)!;
    public static string MarkUnread => ResourceManager.GetString(nameof(MarkUnread), CultureInfo.CurrentUICulture)!;
    public static string MinutesAgo => ResourceManager.GetString(nameof(MinutesAgo), CultureInfo.CurrentUICulture)!;
    public static string MoreToasts => ResourceManager.GetString(nameof(MoreToasts), CultureInfo.CurrentUICulture)!;
    public static string MoveLeft => ResourceManager.GetString(nameof(MoveLeft), CultureInfo.CurrentUICulture)!;
    public static string MoveRight => ResourceManager.GetString(nameof(MoveRight), CultureInfo.CurrentUICulture)!;
    public static string New => ResourceManager.GetString(nameof(New), CultureInfo.CurrentUICulture)!;
    public static string NewItem => ResourceManager.GetString(nameof(NewItem), CultureInfo.CurrentUICulture)!;
    public static string Next => ResourceManager.GetString(nameof(Next), CultureInfo.CurrentUICulture)!;
    public static string NextMonth => ResourceManager.GetString(nameof(NextMonth), CultureInfo.CurrentUICulture)!;
    public static string No => ResourceManager.GetString(nameof(No), CultureInfo.CurrentUICulture)!;
    public static string NoOpenActions => ResourceManager.GetString(nameof(NoOpenActions), CultureInfo.CurrentUICulture)!;
    public static string NothingArchived => ResourceManager.GetString(nameof(NothingArchived), CultureInfo.CurrentUICulture)!;
    public static string NothingFound => ResourceManager.GetString(nameof(NothingFound), CultureInfo.CurrentUICulture)!;
    public static string Notifications => ResourceManager.GetString(nameof(Notifications), CultureInfo.CurrentUICulture)!;
    public static string Now => ResourceManager.GetString(nameof(Now), CultureInfo.CurrentUICulture)!;
    public static string NumberedList => ResourceManager.GetString(nameof(NumberedList), CultureInfo.CurrentUICulture)!;
    public static string OlderNotifications => ResourceManager.GetString(nameof(OlderNotifications), CultureInfo.CurrentUICulture)!;
    public static string PickADay => ResourceManager.GetString(nameof(PickADay), CultureInfo.CurrentUICulture)!;
    public static string PickAFirstDay => ResourceManager.GetString(nameof(PickAFirstDay), CultureInfo.CurrentUICulture)!;
    public static string PickTheHour => ResourceManager.GetString(nameof(PickTheHour), CultureInfo.CurrentUICulture)!;
    public static string PickTheLastDay => ResourceManager.GetString(nameof(PickTheLastDay), CultureInfo.CurrentUICulture)!;
    public static string PickTheMinutes => ResourceManager.GetString(nameof(PickTheMinutes), CultureInfo.CurrentUICulture)!;
    public static string PickTheSeconds => ResourceManager.GetString(nameof(PickTheSeconds), CultureInfo.CurrentUICulture)!;
    public static string Previous => ResourceManager.GetString(nameof(Previous), CultureInfo.CurrentUICulture)!;
    public static string PreviousMonth => ResourceManager.GetString(nameof(PreviousMonth), CultureInfo.CurrentUICulture)!;
    public static string Progress => ResourceManager.GetString(nameof(Progress), CultureInfo.CurrentUICulture)!;
    public static string Recent => ResourceManager.GetString(nameof(Recent), CultureInfo.CurrentUICulture)!;
    public static string Save => ResourceManager.GetString(nameof(Save), CultureInfo.CurrentUICulture)!;
    public static string SaveOpen => ResourceManager.GetString(nameof(SaveOpen), CultureInfo.CurrentUICulture)!;
    public static string Search => ResourceManager.GetString(nameof(Search), CultureInfo.CurrentUICulture)!;
    public static string Select => ResourceManager.GetString(nameof(Select), CultureInfo.CurrentUICulture)!;
    public static string Status => ResourceManager.GetString(nameof(Status), CultureInfo.CurrentUICulture)!;
    public static string Subtitle => ResourceManager.GetString(nameof(Subtitle), CultureInfo.CurrentUICulture)!;
    public static string SubtitleShort => ResourceManager.GetString(nameof(SubtitleShort), CultureInfo.CurrentUICulture)!;
    public static string Time => ResourceManager.GetString(nameof(Time), CultureInfo.CurrentUICulture)!;
    public static string To => ResourceManager.GetString(nameof(To), CultureInfo.CurrentUICulture)!;
    public static string Today => ResourceManager.GetString(nameof(Today), CultureInfo.CurrentUICulture)!;
    public static string TryAgain => ResourceManager.GetString(nameof(TryAgain), CultureInfo.CurrentUICulture)!;
    public static string Underline => ResourceManager.GetString(nameof(Underline), CultureInfo.CurrentUICulture)!;
    public static string Unread => ResourceManager.GetString(nameof(Unread), CultureInfo.CurrentUICulture)!;
    public static string Value => ResourceManager.GetString(nameof(Value), CultureInfo.CurrentUICulture)!;
    public static string Yes => ResourceManager.GetString(nameof(Yes), CultureInfo.CurrentUICulture)!;
    public static string Yesterday => ResourceManager.GetString(nameof(Yesterday), CultureInfo.CurrentUICulture)!;
    public static string YouAreAllCaughtUp => ResourceManager.GetString(nameof(YouAreAllCaughtUp), CultureInfo.CurrentUICulture)!;
    public static string IconColorStyle => ResourceManager.GetString(nameof(IconColorStyle), CultureInfo.CurrentUICulture)!;
    public static string IconNaturalColors => ResourceManager.GetString(nameof(IconNaturalColors), CultureInfo.CurrentUICulture)!;
    public static string IconCategoryColors => ResourceManager.GetString(nameof(IconCategoryColors), CultureInfo.CurrentUICulture)!;
    public static string IconColorIntensity => ResourceManager.GetString(nameof(IconColorIntensity), CultureInfo.CurrentUICulture)!;
    public static string IconNormalIntensity => ResourceManager.GetString(nameof(IconNormalIntensity), CultureInfo.CurrentUICulture)!;
    public static string IconQuietIntensity => ResourceManager.GetString(nameof(IconQuietIntensity), CultureInfo.CurrentUICulture)!;
    public static string IconNavigationSize => ResourceManager.GetString(nameof(IconNavigationSize), CultureInfo.CurrentUICulture)!;
    public static string IconSubtleDepth => ResourceManager.GetString(nameof(IconSubtleDepth), CultureInfo.CurrentUICulture)!;
    public static string IconEntities => ResourceManager.GetString(nameof(IconEntities), CultureInfo.CurrentUICulture)!;
    public static string IconActions => ResourceManager.GetString(nameof(IconActions), CultureInfo.CurrentUICulture)!;
    public static string IconActionStyle => ResourceManager.GetString(nameof(IconActionStyle), CultureInfo.CurrentUICulture)!;
    public static string IconMonochrome => ResourceManager.GetString(nameof(IconMonochrome), CultureInfo.CurrentUICulture)!;
    public static string IconFunctional => ResourceManager.GetString(nameof(IconFunctional), CultureInfo.CurrentUICulture)!;
    public static string IconColored => ResourceManager.GetString(nameof(IconColored), CultureInfo.CurrentUICulture)!;
    public static string IconNaturalMaterials => ResourceManager.GetString(nameof(IconNaturalMaterials), CultureInfo.CurrentUICulture)!;
    public static string IconDestructiveColor => ResourceManager.GetString(nameof(IconDestructiveColor), CultureInfo.CurrentUICulture)!;
}
