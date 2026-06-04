using MyTelegram.Messenger.Handlers.LayerN.Interfaces.Help;
using MyTelegram.Schema.Help.LayerN;

namespace MyTelegram.Messenger.Handlers.LayerN.Impl.Help;
public class TestHandler : RpcResultObjectHandler<MyTelegram.Schema.Help.LayerN.RequestTest, IBool>, ITestHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput request, RequestTest obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
