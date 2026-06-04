using Microsoft.Extensions.Configuration;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace MyTelegram.Services.Services;

public sealed class AccessHashHelper2(
    IConfiguration configuration,
    IPeerHelper peerHelper) : IAccessHashHelper2, ISingletonDependency
{
    private byte[]? _accessHashSecretKeyBytes;

    public Task<bool> IsAccessHashValidAsync(IRequestWithAccessHashKeyId request, long targetId, long accessHash,
        AccessHashType? accessHashType = null)
    {
        return IsAccessHashValidAsync(request.UserId, request.AccessHashKeyId, targetId, accessHash, accessHashType);
    }

    public async Task CheckAccessHashAsync(long currentUserId, long accessHashKeyId, long targetId, long accessHash, AccessHashType? accessHashType = null)
    {
        if (!await IsAccessHashValidAsync(currentUserId, accessHashKeyId, targetId, accessHash, accessHashType))
        {
            switch (accessHashType)
            {
                case AccessHashType.WallPaper:
                    RpcErrors.RpcErrors400.WallpaperInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Theme:
                    RpcErrors.RpcErrors400.ThemeInvalid.ThrowRpcError();
                    break;
                case AccessHashType.GroupCall:
                    RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
                    break;
                case AccessHashType.StickerSet:
                    RpcErrors.RpcErrors400.StickersetInvalid.ThrowRpcError();
                    break;
                case AccessHashType.User:
                    RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Channel:
                    RpcErrors.RpcErrors400.ChannelIdInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Document:
                    RpcErrors.RpcErrors400.DocumentInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Photo:
                    RpcErrors.RpcErrors400.PhotoInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Sticker:
                    RpcErrors.RpcErrors400.StickersetInvalid.ThrowRpcError();
                    break;
                default:
                    RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
                    break;
            }
        }
    }

    public Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, long targetId, long accessHash,
        AccessHashType? accessHashType = null)
    {
        return CheckAccessHashAsync(request.UserId, request.AccessHashKeyId, targetId, accessHash, accessHashType);
    }

    public Task CheckAccessHashAsync(long currentUserId, long accessHashKeyId, IInputPeer? inputPeer)
    {
        switch (inputPeer)
        {
            case null:
                break;
            case TInputPeerChannel inputPeerChannel:
                return CheckAccessHashAsync(currentUserId, accessHashKeyId, inputPeerChannel.ChannelId, inputPeerChannel.AccessHash, AccessHashType.Channel);

            case TInputPeerChannelFromMessage inputPeerChannelFromMessage:

                break;
            case TInputPeerSelf:
                break;
            case TInputPeerUser inputPeerUser:
                return CheckAccessHashAsync(currentUserId, accessHashKeyId, inputPeerUser.UserId, inputPeerUser.AccessHash, AccessHashType.User);

            case TInputPeerUserFromMessage inputPeerUserFromMessage:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputPeer));
        }

        return Task.CompletedTask;
    }

    public Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputPeer? inputPeer)
    {
        return CheckAccessHashAsync(request.UserId, request.AccessHashKeyId, inputPeer);
    }

    public Task CheckAccessHashAsync(long currentUserId, long accessHashKeyId, IInputUser inputUser)
    {
        if (inputUser is TInputUser tInputUser)
        {
            return CheckAccessHashAsync(currentUserId, accessHashKeyId, tInputUser.UserId, tInputUser.AccessHash, AccessHashType.User);
        }

        return Task.CompletedTask;
    }

    public Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputUser inputUser)
    {
        return CheckAccessHashAsync(request.UserId, request.AccessHashKeyId, inputUser);
    }

    public Task CheckAccessHashAsync(long currentUserId, long accessHashKeyId, IInputChannel inputChannel)
    {
        if (inputChannel is TInputChannel tInputChannel)
        {
            return CheckAccessHashAsync(currentUserId, accessHashKeyId, tInputChannel.ChannelId, tInputChannel.AccessHash, AccessHashType.Channel);
        }

        return Task.CompletedTask;
    }

    public Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputChannel inputChannel)
    {
        return CheckAccessHashAsync(request.UserId, request.AccessHashKeyId, inputChannel);
    }

    public Task<bool> IsAccessHashValidAsync(long currentUserId, long accessHashKeyId, long targetId, long accessHash, AccessHashType? accessHashType = AccessHashType.Unknown)
    {
        if (accessHashType == null)
        {
            var peer = peerHelper.GetPeer(targetId);
            switch (peer.PeerType)
            {
                case PeerType.Channel:
                    accessHashType = AccessHashType.Channel;
                    break;
                case PeerType.User:
                    accessHashType = AccessHashType.User;
                    break;
                case PeerType.Self:
                    return Task.FromResult(true);
            }
        }

        if (accessHashType == null)
        {
            return Task.FromResult(false);
        }

        var accessHashForCurrentSession = GenerateAccessHash(currentUserId, accessHashKeyId, targetId, accessHashType.Value);

        return Task.FromResult(accessHashForCurrentSession == accessHash);
    }

    private void GenerateAccessHashSecretKeyForUser(ReadOnlySpan<byte> accessHasKeyBytesOfUser, Span<byte> destination)
    {
        if (_accessHashSecretKeyBytes == null)
        {
            var masterKey = configuration.GetValue<string>("App:AccessHashSecretKey");
            if (string.IsNullOrEmpty(masterKey))
            {
                throw new InvalidOperationException("App:AccessHashSecretKey is null");
            }

            _accessHashSecretKeyBytes = Encoding.UTF8.GetBytes(masterKey);
        }

        HMACSHA256.HashData(_accessHashSecretKeyBytes, accessHasKeyBytesOfUser, destination);
    }

    public long GenerateAccessHash(long currentUserId, long accessHashKeyId, long targetId, AccessHashType accessHashType)
    {
        // accessHashType:1 + currentUserId:8 + targetId:8 + hash:32 + keyForUser:32 + accessHashKeyForUser:8 = 89 bytes
        Span<byte> bytes = stackalloc byte[89];
        bytes[0] = (byte)accessHashType;
        BinaryPrimitives.WriteInt64LittleEndian(bytes.Slice(1, 8), currentUserId);
        BinaryPrimitives.WriteInt64LittleEndian(bytes[17..], targetId);
        var dest = bytes.Slice(17, 32);
        var accessHashSecretKey = bytes.Slice(49, 32);
        var accessHashKeyBytes = bytes[..^8];
        BinaryPrimitives.WriteInt64LittleEndian(accessHashKeyBytes, accessHashKeyId);
        //accessHashKey.TryWriteBytes(accessHashKeyBytes);
        GenerateAccessHashSecretKeyForUser(accessHashKeyBytes, accessHashSecretKey);
        HMACSHA256.HashData(accessHashSecretKey, bytes[..17], dest);

        return BitConverter.ToInt64(dest);
    }
}