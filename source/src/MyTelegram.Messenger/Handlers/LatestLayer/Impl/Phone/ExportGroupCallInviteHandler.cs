using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.Schema.Phone;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Get an <a href="https://corefork.telegram.org/api/links#video-chat-livestream-links">invite link</a> for a group call or livestream
/// <para>Possible errors</para>
/// Code Type Description
/// 403 PUBLIC_CHANNEL_MISSING You can only export group call invite links for public chats or channels.
/// See <a href="https://corefork.telegram.org/method/phone.exportGroupCallInvite" />
///</summary>
internal sealed class ExportGroupCallInviteHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestExportGroupCallInvite, MyTelegram.Schema.Phone.IExportedGroupCallInvite>,
    Phone.IExportGroupCallInviteHandler
{
    protected override async Task<MyTelegram.Schema.Phone.IExportedGroupCallInvite> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestExportGroupCallInvite obj)
    {
        long callId;
        if (obj.Call is TInputGroupCall inputGroupCall)
        {
            callId = inputGroupCall.Id;
        }
        else
        {
            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
            return null!;
        }

        var groupCallReadModel = await queryProcessor.ProcessAsync(
            new GetGroupCallByIdQuery(GroupCallId.Create(callId).Value),
            default);

        if (groupCallReadModel == null)
        {
            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
        }

        if (groupCallReadModel!.IsDiscarded)
        {
            RpcErrors.RpcErrors403.GroupcallForbidden.ThrowRpcError();
        }

        // Generate invite link
        // Format: https://t.me/+{base64_encoded_invite_hash}
        var inviteHash = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"groupcall_{callId}_{groupCallReadModel.AccessHash}"))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        var inviteLink = $"https://t.me/+{inviteHash}";

        return new TExportedGroupCallInvite
        {
            Link = inviteLink
        };
    }
}
