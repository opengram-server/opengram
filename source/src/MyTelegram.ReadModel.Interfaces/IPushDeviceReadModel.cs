// ReSharper Disable All
namespace MyTelegram.ReadModel;

public interface IPushDeviceReadModel : IReadModel
{
    /// <summary>
    ///     If (boolTrue) is transmitted, a sandbox-certificate will be used during transmission.
    /// </summary>
    bool AppSandbox { get; }

    long PermAuthKeyId { get; }
    string Id { get; }

    /// <summary>
    ///     Avoid receiving (silent and invisible background) notifications. Useful to save battery.
    /// </summary>
    bool NoMuted { get; }

    /// <summary>
    ///     List of user identifiers of other users currently using the client
    /// </summary>
    IReadOnlyList<long>? OtherUids { get; }

    /// <summary>
    ///     For FCM and APNS VoIP, optional encryption key used to encrypt push notifications
    /// </summary>
    byte[]? Secret { get; }

    /// <summary>
    ///     Device token
    /// </summary>
    string Token { get; }

    //    1 - APNS(device token for apple push)
    //    2 - FCM(firebase token for google firebase)
    //    3 - MPNS(channel URI for microsoft push)
    //    4 - Deprecated: Simple push(endpoint for firefox's simple push API)
    //    5 - Ubuntu phone (token for ubuntu push)
    //    6 - Blackberry(token for blackberry push)
    //    7 - MTProto separate session
    //    8 - WNS(windows push)
    //    9 - APNS VoIP(token for apple push VoIP)
    //    10 - Web push(web push, see below)
    //    11 - MPNS VoIP(token for microsoft push VoIP)
    //    12 - Tizen(token for tizen push)
    //For 10 web push, the token must be a JSON-encoded object with the following keys:

    //endpoint: Absolute URL exposed by the push service where the application server can send push messages
    //    keys: P-256 elliptic curve Diffie-Hellman parameters in the following object
    //    p256dh: Base64url-encoded P-256 elliptic curve Diffie-Hellman public key
    //    auth: Base64url-encoded authentication secret
    int TokenType { get; }

    long UserId { get; }
    //IReadOnlyList<long> OtherUids { get; }
}