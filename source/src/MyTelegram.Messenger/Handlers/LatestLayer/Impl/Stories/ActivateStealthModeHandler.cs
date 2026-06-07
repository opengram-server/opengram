namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Activates stories stealth mode, see here for more info.
/// See <a href="https://corefork.telegram.org/method/stories.activateStealthMode" />
///</summary>
internal sealed class ActivateStealthModeHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestActivateStealthMode, MyTelegram.Schema.IUpdates>,
    Stories.IActivateStealthModeHandler
{
    protected override Task<IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestActivateStealthMode obj)
    {
        // Stealth mode hides that the user viewed stories for 25 minutes
        // and also hides views from the past 5 minutes.
        // Per corefork.telegram.org: returns updates with storiesStealthMode constructor
        var now = CurrentDate;

        return Task.FromResult<IUpdates>(new TUpdates
        {
            Updates = new TVector<IUpdate>(new List<IUpdate>
            {
                new TUpdateStoriesStealthMode
                {
                    StealthMode = new TStoriesStealthMode
                    {
                        ActiveUntilDate = now + 25 * 60,  // 25 minutes
                        CooldownUntilDate = now + 60 * 60  // 1 hour cooldown
                    }
                }
            }),
            Users = [],
            Chats = [],
            Date = now
        });
    }
}
