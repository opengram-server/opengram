namespace MyTelegram.Domain.CommandHandlers.User;

public class ReviewFrozenAppealCommandHandler : CommandHandler<UserAggregate, UserId, ReviewFrozenAppealCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, ReviewFrozenAppealCommand command, CancellationToken cancellationToken)
    {
        aggregate.ReviewFrozenAppeal(
            command.RequestInfo,
            command.AppealId,
            command.Status,
            command.ModeratorUserId,
            command.ReviewNote);

        return Task.CompletedTask;
    }
}
