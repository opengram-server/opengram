namespace MyTelegram.Domain.CommandHandlers.Channel;

public class
    EditChannelAdminCommand2Handler : CommandHandler<ChannelMemberAggregate, ChannelMemberId, EditChannelAdminCommand2>
{
    public override Task ExecuteAsync(ChannelMemberAggregate aggregate, EditChannelAdminCommand2 command,
        CancellationToken cancellationToken)
    {
        aggregate.EditChannelAdmin2(command.RequestInfo, command.ChannelId, command.UserId, command.AdminRights, command.Rank);

        return Task.CompletedTask;
    }
}