namespace MyTelegram.Domain.CommandHandlers.Temp;

public class StartInviteToChannelCommandHandler : CommandHandler<TempAggregate, TempId, StartInviteToChannelCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartInviteToChannelCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartInviteToChannel(command.RequestInfo, command.ChannelId, command.IsBroadcast, command.HasLink, command.InviterId, command.ChannelHistoryMinId, command.MaxMessageId,
            command.MemberUserIds, command.BotUserIds, command.ChatJoinType);

        return Task.CompletedTask;
    }
}