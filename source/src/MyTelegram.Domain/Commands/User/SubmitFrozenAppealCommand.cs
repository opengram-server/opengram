namespace MyTelegram.Domain.Commands.User;

public class SubmitFrozenAppealCommand : RequestCommand2<UserAggregate, UserId, IExecutionResult>
{
    public SubmitFrozenAppealCommand(
        UserId aggregateId,
        RequestInfo requestInfo,
        string appealId,
        string appealText,
        string captchaToken,
        string userName,
        Dictionary<string, string> answers) : base(aggregateId, requestInfo)
    {
        AppealId = appealId;
        AppealText = appealText;
        CaptchaToken = captchaToken;
        UserName = userName;
        Answers = answers;
    }

    public string AppealId { get; }
    public string AppealText { get; }
    public string CaptchaToken { get; }
    public string UserName { get; }
    public Dictionary<string, string> Answers { get; }
}
