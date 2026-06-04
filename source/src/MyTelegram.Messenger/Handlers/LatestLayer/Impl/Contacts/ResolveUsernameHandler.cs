using MyTelegram.Queries.User;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Contacts;

///<summary>
/// Разрешает @username и возвращает информацию о peer
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CONNECTION_LAYER_INVALID Layer invalid.
/// 400 USERNAME_INVALID The provided username is not valid.
/// 400 USERNAME_NOT_OCCUPIED The provided username is not occupied.
/// See <a href="https://corefork.telegram.org/method/contacts.resolveUsername" />
///</summary>
internal sealed class ResolveUsernameHandler(
    IQueryProcessor queryProcessor,
    //ILayeredService<IChatConverter> layeredChatService,
    //ILayeredService<IUserConverter> layeredUserService,
    IChatConverterService chatConverterService,
    IUserConverterService userConverterService,
    IChannelAppService channelAppService,
    IPhotoAppService photoAppService,
    ILogger<ResolveUsernameHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Contacts.RequestResolveUsername,
            MyTelegram.Schema.Contacts.IResolvedPeer>,
        Contacts.IResolveUsernameHandler
{
    protected override async Task<IResolvedPeer> HandleCoreAsync(IRequestInput input,
        RequestResolveUsername obj)
    {
        if (!string.IsNullOrEmpty(obj.Username))
        {
            var userNameReadModel = await queryProcessor
                .ProcessAsync(new GetUserNameByNameQuery(obj.Username), default);
            
            // Если в основной таблице username не нашли, ищем среди коллекционных username (поле Usernames)
            if (userNameReadModel == null)
            {
                var userWithCollectible = await queryProcessor
                    .ProcessAsync(new GetUserByCollectibleUsernameQuery(obj.Username), default);

                if (userWithCollectible != null)
                {
                    logger.LogDebug("Found collectible username {Username}: UserId={UserId}",
                        obj.Username, userWithCollectible.UserId);

                    // Сразу обрабатываем коллекционный username
                    var user = await userConverterService.GetUserAsync(input, userWithCollectible.UserId, false, false, input.Layer);
                    return new TResolvedPeer
                    {
                        Chats = [],
                        Peer = new TPeerUser { UserId = userWithCollectible.UserId },
                        Users = new TVector<IUser>(user)
                    };
                }
            }
            
            if (userNameReadModel != null)
            {
                logger.LogDebug("Found username {Username}: PeerId={PeerId}, PeerType={PeerType} (int={PeerTypeInt})",
                    obj.Username, userNameReadModel.PeerId, userNameReadModel.PeerType, (int)userNameReadModel.PeerType);
                
                switch (userNameReadModel.PeerType)
                {
                    case PeerType.User:
                        var user = await userConverterService.GetUserAsync(input, userNameReadModel.PeerId, false, false, input.Layer);
                        return new TResolvedPeer
                        {
                            Chats = [],
                            Peer = new TPeerUser { UserId = userNameReadModel.PeerId },
                            Users = new TVector<IUser>(user)
                        };

                    case PeerType.Channel:
                        {
                            var channelReadModel = await channelAppService.GetAsync(userNameReadModel.PeerId);

                            if (channelReadModel != null)
                            {
                                var photoReadModel = await photoAppService.GetAsync(channelReadModel.PhotoId);
                                var channelMemberReadModel = await queryProcessor.ProcessAsync(new GetChannelMemberByUserIdQuery(channelReadModel.ChannelId, input.UserId));
                                return new TResolvedPeer
                                {
                                    Chats =
                                        new TVector<IChat>(chatConverterService.ToChannel(
                                            input,
                                            channelReadModel,
                                            photoReadModel,
                                            channelMemberReadModel, channelMemberReadModel?.Left ?? true, input.Layer)),
                                    Peer = new TPeerChannel { ChannelId = userNameReadModel.PeerId },
                                    Users = []
                                };
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        RpcErrors.RpcErrors400.UsernameNotOccupied.ThrowRpcError();

        return null!;
    }
}
