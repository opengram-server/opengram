using EventFlow.MongoDB.ReadStores;
using EventFlow.MongoDB.ReadStores.Attributes;
using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Events.StarGift;

namespace MyTelegram.ReadModel.Impl;

[MongoDbCollectionName("eventflow-stargiftreadmodel")]
public class StarGiftReadModel : IStarGiftReadModel, IMongoDbReadModel,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftSentEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftSavedEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftConvertedEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftUpgradedEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftTransferredEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftPinnedToggledEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftOutboxMessageCreatedEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftInboxMessageCreatedEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftListedForResaleEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftRemovedFromResaleEvent>,
    IAmReadModelFor<StarGiftAggregate, StarGiftId, StarGiftSoldViaResaleEvent>
{
    public string Id { get; private set; } = null!;
    public string AggregateId { get; private set; } = null!; // настоящий id агрегата в формате "stargift-[GUID]"
    public long GiftId { get; private set; }
    public long FromUserId { get; private set; }
    public long ToUserId { get; private set; }
    public long? ToPeerId { get; private set; }
    public int MessageId { get; private set; }
    public int? OutboxMessageId { get; private set; }
    public long Stars { get; private set; }
    public long ConvertStars { get; private set; }
    public string? Message { get; private set; }
    public bool NameHidden { get; private set; }
    public bool Saved { get; private set; }
    public bool Converted { get; private set; }
    public bool Upgraded { get; private set; }
    public bool Refunded { get; private set; }
    public bool CanUpgrade { get; private set; }
    public bool Pinned { get; private set; }
    public long? SavedId { get; private set; }
    public long? CollectibleId { get; private set; }  // starGiftUnique.id - используется для emoji-статуса
    public long? UpgradeStars { get; private set; }
    public int? UpgradeMsgId { get; private set; }
    public int Date { get; private set; }
    public int? ConvertDate { get; private set; }
    public int? UpgradeDate { get; private set; }
    public byte[]? GiftSticker { get; private set; }
    public long? StickerDocumentId { get; private set; }
    public string? Title { get; private set; }
    public string? UniqueId { get; private set; }
    public byte[]? Attributes { get; private set; }
    public string? UniqueSlug { get; set; }  // slug уникального подарка (например, "PlushPepe-1234")
    public int? UniqueNum { get; set; }      // порядковый номер уникального подарка
    public long? Version { get; set; }
    
    // Поля, связанные с перепродажей
    public bool ForResale { get; set; }
    public long? ResaleStars { get; set; }
    public int? ResaleDate { get; set; }
    public int? SoldDate { get; set; }
    public long? BoughtByUserId { get; set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftSentEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        // Используем AggregateId как основной id, чтобы гарантировать уникальность записи.
        // Так у каждого подарка своя запись, даже если они отправлены одному пользователю одновременно.
        Id = domainEvent.AggregateIdentity.Value; // id агрегата (например, "stargift-{guid}")
        AggregateId = domainEvent.AggregateIdentity.Value; // сохраняем настоящий id агрегата (сейчас совпадает с Id)
        GiftId = domainEvent.AggregateEvent.GiftId;
        FromUserId = domainEvent.AggregateEvent.FromUserId;
        ToUserId = domainEvent.AggregateEvent.ToUserId;
        ToPeerId = domainEvent.AggregateEvent.ToPeerId;
        MessageId = domainEvent.AggregateEvent.MessageId;
        Stars = domainEvent.AggregateEvent.Stars;
        ConvertStars = domainEvent.AggregateEvent.ConvertStars;
        Message = domainEvent.AggregateEvent.Message;
        NameHidden = domainEvent.AggregateEvent.NameHidden;
        CanUpgrade = domainEvent.AggregateEvent.CanUpgrade;
        UpgradeStars = domainEvent.AggregateEvent.UpgradeStars;
        GiftSticker = domainEvent.AggregateEvent.GiftSticker;
        
        // Извлекаем StickerDocumentId из байтов GiftSticker
        if (domainEvent.AggregateEvent.GiftSticker != null && domainEvent.AggregateEvent.GiftSticker.Length >= 8)
        {
            StickerDocumentId = BitConverter.ToInt64(domainEvent.AggregateEvent.GiftSticker, 0);
            Console.WriteLine($"[StarGiftReadModel] Extracted StickerDocumentId={StickerDocumentId} from GiftSticker bytes");
        }
        else
        {
            Console.WriteLine($"[StarGiftReadModel] GiftSticker is null or too short! Length: {domainEvent.AggregateEvent.GiftSticker?.Length ?? 0}");
        }

        Date = domainEvent.AggregateEvent.Date;

        // По умолчанию подарки видны в профиле (Saved = true).
        // Пользователь может скрыть их через payments.saveStarGift(save=false).
        Saved = true;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftSavedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Saved = domainEvent.AggregateEvent.Saved;
        SavedId = domainEvent.AggregateEvent.SavedId;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftConvertedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Converted = true;
        ConvertDate = domainEvent.AggregateEvent.ConvertDate;
        Saved = false;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftUpgradedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[StarGiftReadModel] Applying StarGiftUpgradedEvent. Id={Id}, GiftId={GiftId}");
        Upgraded = true;
        UpgradeMsgId = domainEvent.AggregateEvent.UpgradeMsgId;
        UpgradeDate = domainEvent.AggregateEvent.UpgradeDate;
        UniqueId = domainEvent.AggregateEvent.UniqueId;
        Attributes = domainEvent.AggregateEvent.Attributes;
        
        // CollectibleId выставляется напрямую в UpgradeStarGiftHandler через обновление в MongoDB.
        // Здесь его не вычисляем, чтобы избежать гонок.
        // Формула в UpgradeStarGiftHandler: Math.Abs((long)giftInstanceId.GetHashCode()) + (giftId * 1000000)

        Console.WriteLine($"[StarGiftReadModel] StarGiftUpgradedEvent applied. Upgraded={Upgraded}, UniqueId={UniqueId}, CollectibleId={CollectibleId}, Attributes={Attributes?.Length ?? 0} bytes");

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftTransferredEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        ToUserId = domainEvent.AggregateEvent.NewOwnerId;
        Saved = false;
        Pinned = false;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftPinnedToggledEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Pinned = domainEvent.AggregateEvent.Pinned;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftListedForResaleEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        ForResale = true;
        ResaleStars = domainEvent.AggregateEvent.ResaleStars;
        ResaleDate = domainEvent.AggregateEvent.ResaleDate;
        Console.WriteLine($"[StarGiftReadModel] Gift listed for resale: Id={Id}, Price={ResaleStars} stars");

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftRemovedFromResaleEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        ForResale = false;
        ResaleStars = null;
        Console.WriteLine($"[StarGiftReadModel] Gift removed from resale: Id={Id}");

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftSoldViaResaleEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        // Подарки на витрине перепродажи обязаны быть улучшенными (коллекционными)
        if (!Upgraded)
        {
            Console.WriteLine($"[StarGiftReadModel] WARNING: Gift {Id} sold via resale but not upgraded! This is a data inconsistency - gifts on resale must be collectible.");
            Console.WriteLine($"[StarGiftReadModel] GiftId={GiftId}, Upgraded={Upgraded}, CollectibleId={CollectibleId}");
            // По правилам Telegram API на перепродаже могут быть только улучшенные подарки.
            // Такого быть не должно, поэтому просто пишем в лог для отладки.
        }

        ForResale = false;
        SoldDate = domainEvent.AggregateEvent.SoldDate;
        BoughtByUserId = domainEvent.AggregateEvent.BoughtByUserId;
        ToUserId = domainEvent.AggregateEvent.BoughtByUserId; // передаём право владения
        // Автоматически сохраняем коллекционные подарки, купленные на перепродаже.
        // Когда кто-то покупает Вам дорогой уникальный подарок, он должен сразу появиться в профиле.
        // Обычные подарки требуют ручного сохранения, а коллекционные с витрины показываем сами.
        Saved = true; // автосохранение подарков с перепродажи (это ценные коллекционные предметы)
        Pinned = false; // сбрасываем статус закрепления
        // Upgraded оставляем как есть (для подарков с перепродажи он уже должен быть true)

        // Не обновляем UniqueSlug после перепродажи.
        // UniqueSlug задаётся один раз при улучшении (например, "Cat-8") и должен оставаться постоянным.
        // Его изменение сломало бы ссылки на NFT и сделало бы подарок неузнаваемым.
        // Формат slug "ИмяПодарка-ПорядковыйНомер" - часть идентичности подарка.

        Console.WriteLine($"[StarGiftReadModel] Gift sold via resale: Id={Id}, GiftId={GiftId}, Buyer={BoughtByUserId}, Price={domainEvent.AggregateEvent.ResaleStars} stars, Upgraded={Upgraded}, UniqueSlug={UniqueSlug} (unchanged)");

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftOutboxMessageCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        OutboxMessageId = domainEvent.AggregateEvent.OutboxMessageId;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftInboxMessageCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        MessageId = domainEvent.AggregateEvent.InboxMessageId;
        return Task.CompletedTask;
    }
}
