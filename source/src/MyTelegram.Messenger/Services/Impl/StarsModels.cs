using System.Buffers;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Services.Impl
{
    public class StarsStatus
    {
        public long Balance { get; set; }
        public bool IsBot { get; set; }
        public DateTime NextOffset { get; set; }
    }

    public class StarsTransaction
    {
        public long Id { get; set; }
        public long Amount { get; set; }
        public int Date { get; set; }
        public string Source { get; set; } = string.Empty;
        public StarsTransactionPeer Peer { get; set; } = new();
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? TransactionUrl { get; set; }
        public ReadOnlyMemory<byte>? BotPayload { get; set; }
        public int? MessageId { get; set; }
        public int? SubscriptionPeriod { get; set; }
        public int? GiveawayPostId { get; set; }
        public StarGift? StarGift { get; set; }
        public int? FloodskipNumber { get; set; }
        public int? StarRefCommissionPermille { get; set; }
        public IPeer? StarRefPeer { get; set; }
        public long? StarRefAmount { get; set; }
        public int? PaidMessages { get; set; }
        public int? PremiumGiftMonths { get; set; }
        public int? AdsProceedsFromDate { get; set; }
        public int? AdsProceedsToDate { get; set; }
    }

    public class StarsTransactionPeer
    {
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class StarsSubscription
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Price { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class StarGift
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Price { get; set; }
        public int Count { get; set; }
        public bool IsLimited { get; set; }
    }

    public class StarsTopupOption
    {
        public long Amount { get; set; }
        public int Stars { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    public class StarsGiftOption
    {
        public long Amount { get; set; }
        public int Stars { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}

namespace MyTelegram.Domain.Shared.Business
{
    public enum PeerType
    {
        User,
        Chat,
        Channel
    }

    public class Business
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class Command
    {
        public long Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class PeerSettings
    {
        public bool ReportSpam { get; set; }
        public bool AddContact { get; set; }
        public bool BlockContact { get; set; }
        public bool ShareContact { get; set; }
        public bool InviteMembers { get; set; }

        public PeerSettings Clone()
        {
            return new PeerSettings
            {
                ReportSpam = this.ReportSpam,
                AddContact = this.AddContact,
                BlockContact = this.BlockContact,
                ShareContact = this.ShareContact,
                InviteMembers = this.InviteMembers
            };
        }
    }
}

namespace MyTelegram.Schema.Stars
{
    public class TStarsAmount : IStarsAmount
    {
        public long Amount { get; set; }
        public int Nanos { get; set; }

        public uint ConstructorId => 0x0;
        public void ComputeFlag() { }
        public void Serialize(IBufferWriter<byte> writer) { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
    }
}

namespace MyTelegram.Schema.Updates
{
    public class TUpdateServiceMessage : IUpdate
    {
        public IMessageService Message { get; set; } = null!;

        public uint ConstructorId => 0x0;
        public void ComputeFlag() { }
        public void Serialize(IBufferWriter<byte> writer) { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
    }
}

namespace MyTelegram.Schema.Messages
{
    public class TMessageService : IMessageService, IMessage
    {
        public int Id { get; set; }
        public long PeerId { get; set; }
        public long? FromId { get; set; }
        public IMessageAction Action { get; set; } = null!;
        public int Date { get; set; }
        public int Flags { get; set; }

        public uint ConstructorId => 0x0;
        public void ComputeFlag() { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
        public void Serialize(IBufferWriter<byte> writer) { }
    }

    public interface IMessageService : IMessage
    {
        MyTelegram.Schema.IMessageAction Action { get; set; }
        new int Flags { get; set; }
    }

    public interface IMessageActionChatMigrateTo : MyTelegram.Schema.IMessageAction
    {
        long ChannelId { get; set; }
    }
}

namespace MyTelegram.Domain.Commands
{
    public interface ICommand
    {
    }
}

namespace MyTelegram.Domain.Queries
{
    public interface IQuery
    {
    }
}

namespace MyTelegram.Messenger.QueryServer
{
    public interface IIntegrationEventHandler<T>
    {
        Task HandleAsync(T eventData);
    }
}
