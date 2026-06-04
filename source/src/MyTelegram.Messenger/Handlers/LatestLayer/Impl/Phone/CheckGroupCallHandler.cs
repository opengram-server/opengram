// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.Domain.Aggregates.GroupCall;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Check whether the group call Server Forwarding Unit is currently receiving the streams with the specified WebRTC source IDs.<br>
/// Returns an intersection of the source IDs specified in <code>sources</code>, and the source IDs currently being forwarded by the SFU.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 GROUPCALL_JOIN_MISSING You haven't joined this group call.
/// See <a href="https://corefork.telegram.org/method/phone.checkGroupCall" />
///</summary>
internal sealed class CheckGroupCallHandler(IQueryProcessor queryProcessor) 
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestCheckGroupCall, TVector<int>>,
    Phone.ICheckGroupCallHandler
{
    protected override async Task<TVector<int>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestCheckGroupCall obj)
    {
        // Get call ID
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
        
        // Get group call from read model
        var groupCallReadModel = await queryProcessor.ProcessAsync(
            new GetGroupCallByIdQuery(GroupCallId.Create(callId).Value), 
            default);
        
        if (groupCallReadModel == null)
        {
            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
            return null!;
        }
        
        // Check if user has joined this call
        var userParticipant = groupCallReadModel.Participants
            .FirstOrDefault(p => p.PeerId == input.UserId && !p.Left);
        
        if (userParticipant == null)
        {
            RpcErrors.RpcErrors400.GroupcallJoinMissing.ThrowRpcError();
            return null!;
        }
        
        // Get all active source IDs from the call
        var activeSources = groupCallReadModel.Participants
            .Where(p => !p.Left && p.Source != 0)
            .Select(p => p.Source)
            .ToHashSet();
        
        // Return intersection of requested sources and active sources
        var matchingSources = obj.Sources
            .Where(s => activeSources.Contains(s))
            .ToList();
        
        return new TVector<int>(matchingSources);
    }
}
