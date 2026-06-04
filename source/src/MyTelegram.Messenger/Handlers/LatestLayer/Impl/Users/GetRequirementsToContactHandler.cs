using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Users;
using MyTelegram.Queries;
using MyTelegram.Queries.Privacy;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Users;

///<summary>
/// See <a href="https://corefork.telegram.org/method/users.getRequirementsToContact" />
///</summary>
internal sealed class GetRequirementsToContactHandler(
    IQueryProcessor queryProcessor,
    IContactAppService contactAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Users.RequestGetRequirementsToContact, TVector<MyTelegram.Schema.IRequirementToContact>>,
    IGetRequirementsToContactHandler
{
    protected override async Task<TVector<MyTelegram.Schema.IRequirementToContact>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Users.RequestGetRequirementsToContact obj)
    {
        var requirements = new List<IRequirementToContact>();
        
        // Get current user info
        var currentUser = await queryProcessor.ProcessAsync(new GetUserByIdQuery(input.UserId));
        var isPremium = currentUser?.Premium ?? false;

        foreach (var inputUser in obj.Id)
        {
            if (inputUser is not TInputUser tInputUser)
            {
                requirements.Add(new TRequirementToContactEmpty());
                continue;
            }

            var targetUserId = tInputUser.UserId;
            
            // Get target user
            var targetUser = await queryProcessor.ProcessAsync(new GetUserByIdQuery(targetUserId));
            if (targetUser == null)
            {
                requirements.Add(new TRequirementToContactEmpty());
                continue;
            }

            // Get global privacy settings
            var globalSettings = await queryProcessor.ProcessAsync(new GetGlobalPrivacySettingsQuery(targetUserId));
            
            // Check if user requires premium
            if (globalSettings?.NewNoncontactPeersRequirePremium == true)
            {
                // TODO: Check if we already have contact or existing chat
                // For now, return premium requirement if user doesn't have premium
                if (!isPremium)
                {
                    requirements.Add(new TRequirementToContactPremium());
                    continue;
                }
            }
            
            // Check if user requires paid messages
            if (globalSettings?.NoncontactPeersPaidStars.HasValue == true && globalSettings.NoncontactPeersPaidStars.Value > 0)
            {
                // TODO: Check if we already have contact or existing chat
                requirements.Add(new TRequirementToContactPaidMessages
                {
                    StarsAmount = globalSettings.NoncontactPeersPaidStars.Value
                });
                continue;
            }

            // No requirements
            requirements.Add(new TRequirementToContactEmpty());
        }

        return new TVector<IRequirementToContact>(requirements);
    }
}
