// ReSharper disable All

namespace MyTelegram.Schema.Stories;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/stories.CanSendStoryCount" />
///</summary>
[JsonDerivedType(typeof(TCanSendStoryCount), nameof(TCanSendStoryCount))]
public interface ICanSendStoryCount : IObject
{
    int CountRemains { get; set; }
}
