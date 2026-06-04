namespace MyTelegram.AuthServer.Handlers;

public class MsgsAckHandler : BaseObjectHandler<TMsgsAck, IObject>, IMsgsAckHandler
{
    protected override Task<IObject> HandleCoreAsync(IRequestInput input, TMsgsAck obj)
    {
        return Task.FromResult<IObject>(null!);
    }
}