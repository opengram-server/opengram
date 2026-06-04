// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Invite a set of users to a group call.
/// <para>Possible errors</para>
/// Code Type Description
/// 403 GROUPCALL_FORBIDDEN The group call has already ended.
/// 400 GROUPCALL_INVALID The specified group call is invalid.
/// 400 INVITE_FORBIDDEN_WITH_JOINAS If the user has anonymously joined a group call as a channel, they can't invite other users to the group call because that would cause deanonymization, because the invite would be sent using the original user ID, not the anonymized channel ID.
/// 400 USER_ALREADY_INVITED You have already invited this user.
/// See <a href="https://corefork.telegram.org/method/phone.inviteToGroupCall" />
///</summary>
internal sealed class InviteToGroupCallHandler(
    IQueryProcessor queryProcessor,
    IObjectMessageSender objectMessageSender,
    ILogger<InviteToGroupCallHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestInviteToGroupCall, MyTelegram.Schema.IUpdates>,
    Phone.IInviteToGroupCallHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestInviteToGroupCall obj)
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
            new GetGroupCallByIdQuery(GroupCallId.Create(callId).Value), default);
        
        if (groupCallReadModel == null || groupCallReadModel.IsDiscarded)
        {
            RpcErrors.RpcErrors403.GroupcallForbidden.ThrowRpcError();
        }
        
        var invitedCount = 0;
        foreach (var user in obj.Users)
        {
            if (user is TInputUser inputUser)
            {
                var alreadyParticipant = groupCallReadModel!.Participants
                    .Any(p => p.PeerId == inputUser.UserId && !p.Left);
                
                if (!alreadyParticipant)
                {
                    try
                    {
                        var inviteUpdate = new TUpdateGroupCall
                        {
                            ChatId = groupCallReadModel.PeerId,
                            Call = new TGroupCall
                            {
                                Id = groupCallReadModel.CallId,
                                AccessHash = groupCallReadModel.AccessHash,
                                ParticipantsCount = groupCallReadModel.ParticipantsCount,
                                Title = groupCallReadModel.Title,
                                Version = (int)(groupCallReadModel.Version ?? 0)
                            }
                        };
                        
                        await objectMessageSender.PushMessageToPeerAsync(
                            new Peer(PeerType.User, inputUser.UserId),
                            new TUpdates
                            {
                                Updates = new TVector<IUpdate>(inviteUpdate),
                                Users = new TVector<IUser>(),
                                Chats = new TVector<IChat>(),
                                Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                                Seq = 0
                            },
                            null);
                        invitedCount++;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error inviting userId={UserId}", inputUser.UserId);
                    }
                }
            }
        }
        
        logger.LogInformation("Invited {Count} users to call {CallId}", invitedCount, callId);
        
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };
    }
}
