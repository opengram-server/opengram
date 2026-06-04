using System.Buffers.Binary;
using System.Security.Cryptography;

namespace MyTelegram.Core;

public class AuthKeyIdHelper : IAuthKeyIdHelper, ISingletonDependency
{
    public long GetAuthKeyId(ReadOnlyMemory<byte> authKey)
    {
        Span<byte> hash = stackalloc byte[20];
        SHA1.HashData(authKey.Span, hash);

        return BinaryPrimitives.ReadInt64LittleEndian(hash.Slice(8 + 4));
    }
}