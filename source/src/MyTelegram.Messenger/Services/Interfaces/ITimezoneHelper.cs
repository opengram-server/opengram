namespace MyTelegram.Messenger.Services.Interfaces;

public interface ITimezoneHelper
{
    IReadOnlyCollection<TimeZoneItem> GetTimeZoneList();
    TimeZoneItem? GetTimeZoneItem(string timezoneName);
    bool TryGeTimeZoneItem(string timezoneName, [NotNullWhen(true)] out TimeZoneItem? item);
}