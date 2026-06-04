using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Affiliate;
using MyTelegram.Schema;
using MyTelegram.Schema.Bots;
using MyTelegram.Handlers;
using MyTelegram.Services.Services;
using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Update a <a href="https://corefork.telegram.org/api/bots/referrals">star referral program »</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 BOT_GROUPS_BLOCKED You're not allowed to use bots in groups.
/// 400 BOT_INVALID This is not a valid bot.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 400 USER_BOT_INVALID This user is not a bot.
/// See <a href="https://corefork.telegram.org/method/bots.updateStarRefProgram" />
///</summary>
internal sealed class UpdateStarRefProgramHandler : BaseObjectHandler<RequestUpdateStarRefProgram, IObject>, IUpdateStarRefProgramHandler
{
    private readonly ILogger<UpdateStarRefProgramHandler> logger;
    private readonly IAffiliateAppService affiliateAppService;

    public UpdateStarRefProgramHandler(
        ILogger<UpdateStarRefProgramHandler> logger,
        IAffiliateAppService affiliateAppService)
    {
        this.logger = logger;
        this.affiliateAppService = affiliateAppService;
    }

    protected override async Task<IObject> HandleCoreAsync(IRequestInput input, MyTelegram.Schema.Bots.RequestUpdateStarRefProgram obj)
    {
        logger.LogInformation("Updating star referral program for bot {BotId} by user {UserId}", 
            obj.BotId, input.UserId);

        try
        {
            // Check if program exists
            var existingProgram = await affiliateAppService.GetReferralProgramAsync(obj.BotId);
            
            if (existingProgram == null)
            {
                // Create new program
                logger.LogInformation("Creating new star referral program for bot {BotId}", obj.BotId);

                var createRequest = new CreateReferralProgramRequest
                {
                    BotId = obj.BotId,
                    CreatorId = input.UserId,
                    CommissionPermille = obj.CommissionPermille,
                    DurationMonths = obj.DurationMonths ?? 12
                };

                await affiliateAppService.CreateReferralProgramAsync(createRequest);
            }
            else
            {
                // Update existing program
                logger.LogInformation("Updating existing star referral program for bot {BotId}", obj.BotId);

                var updateRequest = new UpdateReferralProgramRequest
                {
                    UpdatedBy = input.UserId,
                    CommissionPermille = obj.CommissionPermille,
                    DurationMonths = obj.DurationMonths
                };

                await affiliateAppService.UpdateReferralProgramAsync(existingProgram.Id, updateRequest);
            }

            // Return success response
            var result = new TStarRefProgram
            {
                BotId = obj.BotId,
                CommissionPermille = obj.CommissionPermille,
                DurationMonths = obj.DurationMonths ?? 12
            };

            logger.LogInformation("Star referral program updated successfully for bot {BotId}", obj.BotId);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating star referral program for bot {BotId}", obj.BotId);
            throw;
        }
    }
}
