namespace MyTelegram.Core;

public interface IAuthKeyIdHelper
{
    long GetAuthKeyId(ReadOnlyMemory<byte> authKey);
}