using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Changes the <a href="https://corefork.telegram.org/api/business#business-introduction">Telegram Business intro</a> of the current user, installable only by users that have Telegram Premium.
/// See <a href="https://corefork.telegram.org/method/account.updateBusinessIntro" />
///</summary>
internal sealed class UpdateBusinessIntroHandler(
    ILogger<UpdateBusinessIntroHandler> logger,
    ICommandBus commandBus,
    IStickerAppService stickerAppService,
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUpdateBusinessIntro, IBool>,
    Account.IUpdateBusinessIntroHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUpdateBusinessIntro obj)
    {
        logger.LogInformation("Updating business intro for user {UserId}", input.UserId);

        // Check if user has premium/business subscription
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.InputUserDeactivated);
        }

        if (!userReadModel.Premium && !userReadModel.Bot)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PremiumAccountRequired);
        }

        BusinessIntro businessIntro = null;

        if (obj.Intro != null)
        {
            businessIntro = new BusinessIntro
            {
                Title = obj.Intro.Title,
                Description = obj.Intro.Description
            };

            // Validate title and description lengths
            if (string.IsNullOrEmpty(businessIntro.Title) || businessIntro.Title.Length > 70)
            {
                throw new RpcException(RpcErrors.RpcErrors400.TitleInvalid);
            }

            // Description is optional, only validate length if provided
            if (!string.IsNullOrEmpty(businessIntro.Description) && businessIntro.Description.Length > 255)
            {
                throw new RpcException(RpcErrors.RpcErrors400.DataInvalid);
            }

            // Handle sticker document
            if (obj.Intro.Sticker == null || obj.Intro.Sticker is TInputDocumentEmpty)
            {
                logger.LogInformation("Random sticker requested for business intro (Setting ID to 0)");
                // Set to "0" to indicate dynamic random sticker selection in GetFullUser
                businessIntro.StickerDocumentId = "0";
            }
            else if (obj.Intro.Sticker is TInputDocument inputDoc)
            {
                // Specific sticker provided
                // Validate that it's a sticker document
                if (!IsValidStickerDocument(obj.Intro.Sticker))
                {
                    throw new RpcException(RpcErrors.RpcErrors400.StickerInvalid);
                }
                
                businessIntro.StickerDocumentId = inputDoc.Id.ToString();
                logger.LogInformation("Specific sticker {StickerId} set for business intro", inputDoc.Id);
            }
        }

        // Create command to update business intro
        var command = new UpdateBusinessIntroCommand(
            UserId.Create(input.UserId),
            businessIntro);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Business intro updated successfully for user {UserId}", input.UserId);

        return new TBoolTrue();
    }

    private bool IsValidStickerDocument(IInputDocument inputDocument)
    {
        return inputDocument is TInputDocument;
    }
}
