// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/BotVerifierSettings" />
///</summary>
[JsonDerivedType(typeof(TBotVerifierSettings), nameof(TBotVerifierSettings))]
public interface IBotVerifierSettings : IObject
{
    int Flags { get; set; }
    bool CanModifyCustomDescription { get; set; }
    long Icon { get; set; }
    string Company { get; set; }
    string? CustomDescription { get; set; }
}
