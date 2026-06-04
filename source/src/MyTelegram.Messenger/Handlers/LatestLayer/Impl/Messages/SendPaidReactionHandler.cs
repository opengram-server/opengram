// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

using MyTelegram.Domain.Aggregates.Stars;
using MyTelegram.Domain.Commands.Stars;
using global::EventFlow.Commands;
using global::EventFlow.Exceptions;
using MyTelegram.Domain.Commands.Messaging;
using MyTelegram.Domain.Aggregates.Messaging;
using MyTelegram.Schema;
using MyTelegram.Queries.Stars;
using MyTelegram.Services.Extensions;

///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.sendPaidReaction" />
///</summary>
internal sealed class SendPaidReactionHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSendPaidReaction, MyTelegram.Schema.IUpdates>,
    Messages.ISendPaidReactionHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IPtsHelper _ptsHelper;
    private readonly ILogger<SendPaidReactionHandler> _logger;

    public SendPaidReactionHandler(
        ICommandBus commandBus,
        IQueryProcessor queryProcessor,
        IPtsHelper ptsHelper,
        ILogger<SendPaidReactionHandler> logger)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
        _ptsHelper = ptsHelper;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSendPaidReaction obj)
    {
        var peer = obj.Peer.ToPeer();
        var selfUserId = input.UserId;
        var date = DateTime.UtcNow.ToTimestamp();
        var starsAmount = obj.Count;

        _logger.LogInformation(
            "*** SendPaidReaction: PeerType={PeerType}, PeerId={PeerId}, MsgId={MsgId}, Count={Count}",
            peer.PeerType, peer.PeerId, obj.MsgId, obj.Count);

        // 1. Списываем звёзды
        var starsId = StarsId.Create(selfUserId);
        var spendStarsCommand = new SpendStarsCommand(
            starsId,
            input.ToRequestInfo(),
            starsAmount,
            Guid.NewGuid().ToString(),
            $"Paid reaction to {peer.PeerId} msg {obj.MsgId}");

        try
        {
            await _commandBus.PublishAsync(spendStarsCommand, CancellationToken.None);
        }
        catch (DomainError)
        {
            var starsStatus = await _queryProcessor
                .ProcessAsync(new GetStarsStatusQuery(selfUserId), CancellationToken.None);

            if (starsStatus != null && starsStatus.Balance >= starsAmount)
            {
                // 1. Создаём агрегат
                var createCmdId = new CommandId($"command-{Guid.NewGuid()}");
                await _commandBus.PublishAsync(
                    new CreateStarsAccountCommand(starsId, createCmdId, selfUserId),
                    CancellationToken.None);

                // 2. Синхронизируем текущий баланс
                var addCmdId = new CommandId($"command-{Guid.NewGuid()}");
                await _commandBus.PublishAsync(
                    new AddStarsCommand(
                        starsId,
                        addCmdId,
                        input.ToRequestInfo(),
                        starsStatus.Balance,
                        Guid.NewGuid().ToString(),
                        "Sync from ReadModel"),
                    CancellationToken.None);

                // 3. Повторяем списание
                await _commandBus.PublishAsync(spendStarsCommand, CancellationToken.None);
            }
            else
            {
                throw; // пробрасываем исходную ошибку
            }
        }
        catch (RpcException ex) when (ex.RpcError.Message == "BALANCE_TOO_LOW")
        {
            throw;
        }
        
        // Запросу нужен объект реакции, но RequestSendPaidReaction его не передаёт, а IReaction обязателен.
        // Платная реакция по смыслу — это «звезда», поэтому используем эмодзи-реакцию со звездой.
        var reaction = new TReactionEmoji { Emoticon = "⭐" };

        var command = new AddReactionCommand(
            MessageId.Create(peer.PeerId, obj.MsgId),
            input.ToRequestInfo(),
            selfUserId,
            new Peer(PeerType.User, selfUserId),
            reaction,
            false, // платная реакция не отображается как «большая» в анимации
            true,  // добавляем в недавние
            date,
            obj.Count
        );

        await _commandBus.PublishAsync(command, CancellationToken.None);

        // 3. Возвращаем обновления.
        // Нужно вернуть обновлённые реакции сообщения: как и в SendReactionHandler,
        // получаем сообщение и формируем обновления.
        var messageReadModel = await _queryProcessor.ProcessAsync(
            new GetMessageByIdQuery(MessageId.Create(peer.PeerId, obj.MsgId, false).Value),
            CancellationToken.None);

        // Формируем обновление
        var pts = _ptsHelper.GetCachedPts(peer.PeerId);
        var update = new TUpdateMessageReactions
        {
            Peer = peer.ToPeer(),
            MsgId = obj.MsgId,
            Reactions = CreateMessageReactions(messageReadModel)
        };
        
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = date,
            Seq = 0
        };
    }

    private IMessageReactions CreateMessageReactions(IMessageReadModel? message)
    {
        if (message?.Reactions == null || message.Reactions.Count == 0)
        {
            return new TMessageReactions
            {
                Results = new TVector<IReactionCount>(),
                CanSeeList = false
            };
        }

        var results = message.Reactions.Select(r => new TReactionCount
        {
            Reaction = r.Reaction,
            Count = r.Count
        }).ToList();

        // В каналах не показываем, кто поставил реакцию (анонимные реакции).
        // В группах и личных чатах показываем недавние реакции вместе с пользователями.
        var isChannel = message.ToPeerType == PeerType.Channel;
        
        _logger.LogInformation("*** CreateMessageReactions: ToPeerType={ToPeerType}, IsChannel={IsChannel}, RecentReactionsCount={Count}",
            message.ToPeerType, isChannel, message.RecentReactions2?.Count ?? 0);
        
        var recentReactions = !isChannel && message.RecentReactions2 != null
            ? message.RecentReactions2.Select(r => new TMessagePeerReaction
            {
                Big = r.Big,
                PeerId = r.PeerId.ToPeer(),
                Date = r.Date,
                Reaction = r.Reaction
            }).ToList()
            : null;

        return new TMessageReactions
        {
            Results = new TVector<IReactionCount>(results),
            RecentReactions = recentReactions != null ? new TVector<IMessagePeerReaction>(recentReactions) : null,
            CanSeeList = !isChannel && message.Reactions?.Any() == true
        };
    }
}
