namespace MyTelegram.MTProto;

public interface IClientData
{
    public long AuthKeyId { get; set; }

    public string ConnectionId { get; set; }
    public int CurrentPacketLength { get; set; }

    public bool IsFirstPacketParsed { get; set; }
    public ProtocolType MtProtoType { get; set; }
    public bool ObfuscationEnabled { get; set; }
    public byte[] ReceiveKey { get; set; }
    public byte[] SendKey { get; set; }
    public int SkipCount { get; set; }

    public byte[] SendIv { get; set; }
    public byte[] ReceiveIv { get; set; }

    public ulong ReceiveCount { get; set; }
    public ulong SendCount { get; set; }
}

public interface IClientData<T> : IClientData
{
    T Data { get; set; }
}
