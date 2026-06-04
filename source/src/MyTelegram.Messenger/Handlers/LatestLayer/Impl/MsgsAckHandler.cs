// ReSharper disable All
namespace MyTelegram.Handlers.Impl;

internal sealed class MsgsAckHandler : BaseObjectHandler<TMsgsAck, IObject>, IMsgsAckHandler
{
    private readonly ILogger<MsgsAckHandler> _logger;

    public MsgsAckHandler(ILogger<MsgsAckHandler> logger)
    {
        _logger = logger;
    }

    protected override Task<IObject> HandleCoreAsync(IRequestInput input,
        TMsgsAck obj)
    {
        return Task.FromResult<IObject>(null!);
        //IObject r = new TMsgsAck
        //{
        //    MsgIds = new TVector<long>()
        //};

        //return Task.FromResult(r);
    }
}