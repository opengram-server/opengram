using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Updates the <a href="https://corefork.telegram.org/api/business#location">Telegram Business location</a> of the current user, installable only by users that have Telegram Premium.
/// See <a href="https://corefork.telegram.org/method/account.updateBusinessLocation" />
///</summary>
internal sealed class UpdateBusinessLocationHandler(
    ILogger<UpdateBusinessLocationHandler> logger,
    ICommandBus commandBus,
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUpdateBusinessLocation, IBool>,
    Account.IUpdateBusinessLocationHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUpdateBusinessLocation obj)
    {
        logger.LogInformation("Updating business location for user {UserId}", input.UserId);

        // Check if user has premium/business subscription
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            throw new RpcException(new RpcError(400, "USER_DEACTIVATED"));
        }

        if (!userReadModel.Premium && !userReadModel.Bot)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PremiumAccountRequired);
        }

        var businessLocation = new BusinessLocation();

        if (obj.Address != null)
        {
            businessLocation.Address = obj.Address;
            
            // Validate address length
            if (string.IsNullOrEmpty(businessLocation.Address) || businessLocation.Address.Length > 96)
            {
                throw new RpcException(new RpcError(400, "BAD_REQUEST"));
            }
        }

        if (obj.GeoPoint != null)
        {
            // Extract coordinates from GeoPoint if it's InputGeoPoint
            if (obj.GeoPoint is MyTelegram.Schema.TInputGeoPoint geoPoint)
            {
                businessLocation.Latitude = (float?)geoPoint.Lat;
                businessLocation.Longitude = (float?)geoPoint.Long;

                // Validate coordinates
                if (businessLocation.Latitude < -90 || businessLocation.Latitude > 90)
                {
                    throw new RpcException(new RpcError(400, "BAD_REQUEST"));
                }

                if (businessLocation.Longitude < -180 || businessLocation.Longitude > 180)
                {
                    throw new RpcException(new RpcError(400, "BAD_REQUEST"));
                }
            }
        }

        // Create command to update business location
        var command = new UpdateBusinessLocationCommand(
            UserId.Create(input.UserId),
            businessLocation);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Business location updated successfully for user {UserId}", input.UserId);

        return new TBoolTrue();
    }
}
