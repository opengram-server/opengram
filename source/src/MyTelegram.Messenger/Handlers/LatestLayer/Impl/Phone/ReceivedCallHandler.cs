// ReSharper disable All

using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Optional: notify the server that the user is currently busy in a call: this will automatically refuse all incoming phone calls until the current phone call is ended.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CALL_ALREADY_DECLINED The call was already declined.
/// 400 CALL_PEER_INVALID The provided call peer object is invalid.
/// See <a href="https://corefork.telegram.org/method/phone.receivedCall" />
///</summary>
internal sealed class ReceivedCallHandler(ICommandBus commandBus) : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestReceivedCall, IBool>,
    Phone.IReceivedCallHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestReceivedCall obj)
    {
        var phoneCallId = PhoneCallId.Create(obj.Peer.Id);
        var command = new ReceivedCallCommand(
            phoneCallId,
            input.ToRequestInfo(),
            DateTime.UtcNow.ToTimestamp());

        await commandBus.PublishAsync(command, CancellationToken.None);

        return new TBoolTrue();
    }
}
