// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Contains the link that must be used to open a <a href="https://corefork.telegram.org/api/bots/webapps#direct-link-mini-apps">direct link Mini App</a>.
/// See <a href="https://corefork.telegram.org/constructor/AppWebViewResult" />
///</summary>
public interface IAppWebViewResult : IObject
{
    ///<summary>
    /// The URL to open
    ///</summary>
    string Url { get; set; }
}
