using MyTelegram.Schema;

namespace MyTelegram.Schema.Channels
{
    public class RequestCreateMonoforum : IRequest<IObject>
    {
        public long ChannelId { get; set; }

        public uint ConstructorId => 0x12345678;
        public void ComputeFlag() { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
        public void Serialize(IBufferWriter<byte> writer) { }
    }

    public class RequestCreateSuggestedPost : IRequest<IObject>
    {
        public long ChannelId { get; set; }
        public string Message { get; set; } = string.Empty;

        public uint ConstructorId => 0x12345679;
        public void ComputeFlag() { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
        public void Serialize(IBufferWriter<byte> writer) { }
    }
}

namespace MyTelegram.Schema.Account  
{
    public class RequestUpdateStarsRating : IRequest<IObject>
    {
        public long UserId { get; set; }
        public int Rating { get; set; }

        public uint ConstructorId => 0x1234567A;
        public void ComputeFlag() { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
        public void Serialize(IBufferWriter<byte> writer) { }
    }
}

namespace MyTelegram.Schema.Messages
{
    public class RequestCreateChecklist : IRequest<IObject>
    {
        public IInputPeer Peer { get; set; }
        public string Title { get; set; } = string.Empty;
        public TVector<MyTelegram.Schema.IMessageEntity> TitleEntities { get; set; }
        public TVector<MyTelegram.Domain.Shared.Checklists.InputChecklistTask> Tasks { get; set; }

        public uint ConstructorId => 0x1234567B;
        public void ComputeFlag() { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
        public void Serialize(IBufferWriter<byte> writer) { }
    }

    public class RequestCreateDirectMessage : IRequest<IObject>
    {
        public long UserId { get; set; }
        public string Message { get; set; } = string.Empty;

        public uint ConstructorId => 0x1234567C;
        public void ComputeFlag() { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
        public void Serialize(IBufferWriter<byte> writer) { }
    }
}

namespace MyTelegram.Schema.Bots
{
    public class RequestUpdateStarRefProgram : IRequest<IObject>
    {
        public long BotId { get; set; }
        public int CommissionPermille { get; set; }
        public int? DurationMonths { get; set; }

        public uint ConstructorId => 0x1234567D;
        public void ComputeFlag() { }
        public void Deserialize(ref ReadOnlyMemory<byte> buffer) { }
        public void Serialize(IBufferWriter<byte> writer) { }
    }
}

