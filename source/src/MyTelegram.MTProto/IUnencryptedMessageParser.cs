namespace MyTelegram.MTProto;

public interface IUnencryptedMessageParser
{
    UnencryptedMessage Parse(ReadOnlyMemory<byte> data);
}
