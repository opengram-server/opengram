// ReSharper disable All

namespace MyTelegram.Schema.Help;

///<summary>
/// Timezone information that may be used elsewhere in the API, such as to set <a href="https://corefork.telegram.org/api/business#opening-hours">Telegram Business opening hours »</a>.
/// See <a href="https://corefork.telegram.org/constructor/help.TimezonesList" />
///</summary>
[JsonDerivedType(typeof(TTimezonesListNotModified), nameof(TTimezonesListNotModified))]
[JsonDerivedType(typeof(TTimezonesList), nameof(TTimezonesList))]
public interface ITimezonesList : IObject
{

}
