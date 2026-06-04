// ReSharper disable once CheckNamespace

namespace MyTelegram;

/// <summary>
///     The request information
/// </summary>
public interface IRequestInput : IRequestWithAccessHashKeyId
{
    string ConnectionId { get; }

    /// <summary>
    ///     Temp auth key id
    /// </summary>
    long AuthKeyId { get; }

    /// <summary>
    ///     Request object id
    /// </summary>
    uint ObjectId { get; }

    /// <summary>
    ///     Request permanent auth key id
    /// </summary>
    long PermAuthKeyId { get; }

    /// <summary>
    ///     Request message id
    /// </summary>
    long ReqMsgId { get; }
    int SeqNumber { get; }

    ///// <summary>
    /////     Request user id
    ///// </summary>
    //long UserId { get; }

    int Layer { get; }
    Guid RequestId { get; }
    long Date { get; }
    DeviceType DeviceType { get; }
    string ClientIp { get; }
    long SessionId { get; }
    //long AccessHashKeyId { get; }
}

public interface IRequestWithAccessHashKeyId
{
    long UserId { get; }
    long AccessHashKeyId { get; }
}