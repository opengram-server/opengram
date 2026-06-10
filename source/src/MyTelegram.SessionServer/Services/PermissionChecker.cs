using MyTelegram.Abstractions;
using MyTelegram.Core;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Validates whether a session is allowed to invoke a given API.
/// Reconstructed from the original binary at 0x00a07c20.
/// Checks: login-required, 2FA state, media-only connections, unregistered auth keys.
/// </summary>
public sealed class PermissionChecker
{
    private readonly ILogger<PermissionChecker> _logger;

    // In the original, these are FrozenDictionary<uint, T> from ObjectIdConsts.
    // Since the current codebase doesn't expose AuthorizationRequiredObjectIds etc.
    // as public dictionaries yet, we define local sets here. These should be moved
    // to ObjectIdConsts when the full permission map is implemented.

    // APIs that do NOT require a logged-in user (pre-login allowed)
    private static readonly HashSet<uint> _noLoginRequired = new()
    {
        ObjectIdConsts.BindTempAuthKey,      // auth.bindTempAuthKey
        ObjectIdConsts.InvokeWithLayer,      // invokeWithLayer
        ObjectIdConsts.InitConnectionId,     // initConnection
        ObjectIdConsts.MsgAcks,              // msgs_ack
        ObjectIdConsts.PingId,               // ping
        ObjectIdConsts.PingDelayId,          // ping_delay_disconnect
        ObjectIdConsts.MsgContainer,         // msg_container
        ObjectIdConsts.GzipPackedId,         // gzip_packed
        0x3e72ba19,                          // auth.logOut
        0xd18b4d16,                          // auth.checkPassword
        0xcd050916,                          // auth.sendCode
        0xa677244f,                          // auth.resendCode
        0x80eee427,                          // auth.signUp
        0x8d52a951,                          // auth.signIn
        0x1b067634,                          // auth.exportAuthorization
        0xe3ef9613,                          // auth.importAuthorization
    };

    // APIs allowed while 2FA password verification is pending
    private static readonly HashSet<uint> _twoFactorAllowed = new()
    {
        ObjectIdConsts.BindTempAuthKey,
        ObjectIdConsts.InvokeWithLayer,
        ObjectIdConsts.InitConnectionId,
        ObjectIdConsts.MsgAcks,
        ObjectIdConsts.PingId,
        ObjectIdConsts.PingDelayId,
        ObjectIdConsts.MsgContainer,
        ObjectIdConsts.GzipPackedId,
        0xd18b4d16, // auth.checkPassword
        0x3e72ba19, // auth.logOut
        0x4ea56e92, // account.getPassword
    };

    // APIs allowed on Media-type connections
    private static readonly HashSet<uint> _mediaAllowed = new()
    {
        ObjectIdConsts.GetFileObjectId,
        ObjectIdConsts.GetFileObjectIdLayer143,
        ObjectIdConsts.SaveFilePartObjectId,
        ObjectIdConsts.SaveBigFilePartObjectId,
        ObjectIdConsts.UploadMediaObjectId,
        ObjectIdConsts.InvokeWithLayer,
        ObjectIdConsts.InitConnectionId,
        ObjectIdConsts.MsgAcks,
        ObjectIdConsts.PingId,
        ObjectIdConsts.PingDelayId,
        ObjectIdConsts.MsgContainer,
        ObjectIdConsts.GzipPackedId,
        ObjectIdConsts.BindTempAuthKey,
    };

    public PermissionChecker(ILogger<PermissionChecker> logger)
    {
        _logger = logger;
    }

    /// <summary>Returns true if the API does NOT require a logged-in user.</summary>
    public bool IsLoginRequiredApi(uint objectId)
        => !_noLoginRequired.Contains(objectId);

    /// <summary>
    /// Validates whether the session is allowed to invoke the given API.
    /// Returns null if allowed, or an RPC error otherwise.
    /// </summary>
    public Schema.TRpcError? CheckPermission(
        Session session,
        long authKeyId,
        uint objectId,
        ConnectionType connectionType)
    {
        // 1. APIs that do not require login are always allowed
        if (_noLoginRequired.Contains(objectId))
            return null;

        // 2. Waiting for 2FA password: only specific subset allowed
        if (session.PasswordState == PasswordState.WaitingForVerify &&
            !_twoFactorAllowed.Contains(objectId))
        {
            return new Schema.TRpcError
            {
                ErrorCode = 401,
                ErrorMessage = "SESSION_PASSWORD_NEEDED"
            };
        }

        // 3. On Media connections, only whitelisted APIs
        if (connectionType == ConnectionType.Media && !_mediaAllowed.Contains(objectId))
        {
            _logger.LogWarning(
                "API objectId=0x{ObjectId:X} not allowed on Media connection for authKey={AuthKeyId}",
                objectId, authKeyId);
            return new Schema.TRpcError
            {
                ErrorCode = 400,
                ErrorMessage = "CONNECTION_NOT_INITED"
            };
        }

        // 4. Not logged in (UserId == 0): reject authorized-only APIs
        if (session.UserId == 0)
        {
            _logger.LogWarning(
                "Auth required but session not logged in: authKey={AuthKeyId} objectId=0x{ObjectId:X}",
                authKeyId, objectId);
            return new Schema.TRpcError
            {
                ErrorCode = 401,
                ErrorMessage = "AUTH_KEY_UNREGISTERED"
            };
        }

        return null;
    }
}
