// ReSharper disable All

using MyTelegram.Messenger.Services;
using MyTelegram.Domain.Shared.Forums;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Messenger.Converters.ConverterServices;

namespace MyTelegram.Messenger.Handlers.Channels;

///<summary>
/// See <a href="https://corefork.telegram.org/method/channels.updatePaidMessagesPrice" />
///</summary>
internal sealed class UpdatePaidMessagesPriceHandler(
    ICommandBus commandBus,
    IAccessHashHelper accessHashHelper,
    IMonoforumAppService monoforumAppService,
    IChannelAppService channelAppService,
    IChatConverterService chatConverterService,
    IPhotoAppService photoAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Channels.RequestUpdatePaidMessagesPrice, MyTelegram.Schema.IUpdates>,
    Channels.IUpdatePaidMessagesPriceHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Channels.RequestUpdatePaidMessagesPrice obj)
    {
        if (obj.Channel is TInputChannel inputChannel)
        {
            await accessHashHelper.CheckAccessHashAsync(input, inputChannel);
            
            // Check if channel already has a monoforum
            var existingMonoforum = await monoforumAppService.GetMonoforumAsync(inputChannel.ChannelId);
            
            long? monoforumId = existingMonoforum?.Id;
            
            // If enabling paid messages (stars > 0) and no monoforum exists, create one
            if (obj.SendPaidMessagesStars > 0 && existingMonoforum == null)
            {
                var createMonoforumRequest = new CreateMonoforumRequest
                {
                    ChannelId = inputChannel.ChannelId,
                    CreatorId = input.UserId,
                    Title = $"Direct Messages",
                    Description = "Send direct messages to this channel",
                    Settings = new MonoforumSettings
                    {
                        AllowAnonymous = false,
                        RequiresApproval = false,
                        MaxTopicsPerUser = 1,
                        AllowedUserRoles = new List<string> { "member" }
                    }
                };
                
                var monoforum = await monoforumAppService.CreateMonoforumAsync(createMonoforumRequest);
                monoforumId = monoforum.Id;
            }
            
            await commandBus.PublishAsync(
                new UpdatePaidMessagesPriceCommand(
                    ChannelId.Create(inputChannel.ChannelId),
                    input.ToRequestInfo(),
                    obj.SendPaidMessagesStars,
                    true,  // broadcastMessagesAllowed - set to true when paid messages enabled
                    monoforumId));

            var channelReadModel = await channelAppService.GetAsync(inputChannel.ChannelId);
            var photoReadModel = await photoAppService.GetAsync(channelReadModel.PhotoId);
            var chat = chatConverterService.ToChannel(input.ToRequestInfo(), channelReadModel, photoReadModel, null, false, input.Layer);

            if (chat is MyTelegram.Schema.TChannel channel)
            {
                channel.SendPaidMessagesStars = obj.SendPaidMessagesStars;
            }

            return new MyTelegram.Schema.TUpdates
            {
                Updates = new MyTelegram.Schema.TVector<MyTelegram.Schema.IUpdate>(new MyTelegram.Schema.TUpdateChannel { ChannelId = inputChannel.ChannelId }),
                Users = new MyTelegram.Schema.TVector<MyTelegram.Schema.IUser>(),
                Chats = new MyTelegram.Schema.TVector<MyTelegram.Schema.IChat>(chat),
                Date = DateTime.UtcNow.ToTimestamp(),
                Seq = 0
            };
        }

        throw new RpcException(RpcErrors.RpcErrors400.ChannelInvalid);
    }
}
