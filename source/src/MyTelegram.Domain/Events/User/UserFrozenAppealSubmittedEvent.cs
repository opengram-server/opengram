namespace MyTelegram.Domain.Events.User;

public class UserFrozenAppealSubmittedEvent : RequestAggregateEvent2<UserAggregate, UserId>
{
    public long UserId { get; }
    public string AppealId { get; }
    public string AppealText { get; }
    public string CaptchaToken { get; }
    public string UserName { get; }
    public Dictionary<string, string> Answers { get; }

    public UserFrozenAppealSubmittedEvent(
        RequestInfo requestInfo,
        long userId,
        string appealId,
        string appealText,
        string captchaToken,
        string userName,
        Dictionary<string, string> answers) : base(requestInfo)
    {
        UserId = userId;
        AppealId = appealId;
        AppealText = appealText;
        CaptchaToken = captchaToken;
        UserName = userName;
        Answers = answers;
    }
}
