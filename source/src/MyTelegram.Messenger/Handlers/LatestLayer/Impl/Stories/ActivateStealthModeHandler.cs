namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Activates stories stealth mode, see here for more info.
/// Per corefork: requires Premium, hides story views for 25 minutes,
/// also hides views from the past 5 minutes. 1 hour cooldown.
/// See <a href="https://corefork.telegram.org/method/stories.activateStealthMode" />
///</summary>
internal sealed class ActivateStealthModeHandler(
    IUserAppService userAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestActivateStealthMode, MyTelegram.Schema.IUpdates>,
    Stories.IActivateStealthModeHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestActivateStealthMode obj)
    {
        // Per corefork: stealth mode requires a Premium subscription
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
        }

        if (!userReadModel!.Premium)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PremiumAccountRequired);
        }

        // Stealth mode hides that the user viewed stories for 25 minutes
        // and also hides views from the past 5 minutes.
        // Returns updates with storiesStealthMode constructor
        var now = CurrentDate;

        return new TUpdates
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
        };
    }
}
