// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Webpage attributes
/// See <a href="https://corefork.telegram.org/constructor/WebPageAttribute" />
///</summary>
[JsonDerivedType(typeof(TWebPageAttributeTheme), nameof(TWebPageAttributeTheme))]
[JsonDerivedType(typeof(TWebPageAttributeStory), nameof(TWebPageAttributeStory))]
[JsonDerivedType(typeof(TWebPageAttributeStickerSet), nameof(TWebPageAttributeStickerSet))]
[JsonDerivedType(typeof(TWebPageAttributeUniqueStarGift), nameof(TWebPageAttributeUniqueStarGift))]
public interface IWebPageAttribute : IObject
{

}
