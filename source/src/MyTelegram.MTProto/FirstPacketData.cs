namespace MyTelegram.MTProto;

public class FirstPacketData
{
    public bool ObfuscationEnabled { get; set; }
    public ProtocolType ProtocolType { get; set; }
    public byte[] ReceiveKey { get; set; } = [];
    public byte[] ReceiveIv { get; set; } = [];
    public byte[] SendKey { get; set; } = [];
    public byte[] SendIv { get; set; } = [];
    public int ProtocolBufferLength { get; set; }
    public ulong SendCount { get; set; }
}
