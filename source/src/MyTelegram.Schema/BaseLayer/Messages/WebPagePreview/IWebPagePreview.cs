// ReSharper disable All

namespace MyTelegram.Schema.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/messages.WebPagePreview" />
///</summary>
[JsonDerivedType(typeof(TWebPagePreview), nameof(TWebPagePreview))]
public interface IWebPagePreview : IObject
{
    MyTelegram.Schema.IMessageMedia Media { get; set; }
    TVector<MyTelegram.Schema.IUser> Users { get; set; }
}
