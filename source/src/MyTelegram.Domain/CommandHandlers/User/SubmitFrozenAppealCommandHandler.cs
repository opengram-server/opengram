namespace MyTelegram.Domain.CommandHandlers.User;

public class SubmitFrozenAppealCommandHandler : CommandHandler<UserAggregate, UserId, SubmitFrozenAppealCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, SubmitFrozenAppealCommand command, CancellationToken cancellationToken)
    {
        aggregate.SubmitFrozenAppeal(
            command.RequestInfo,
            command.AppealId,
            command.AppealText,
            command.CaptchaToken,
            command.UserName,
            command.Answers);

        return Task.CompletedTask;
    }
}
