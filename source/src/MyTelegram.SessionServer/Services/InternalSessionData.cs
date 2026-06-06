using MyTelegram.Core;
using MyTelegram.Schema;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Wraps a deserialized RPC request together with its <see cref="IRequestInput"/>
/// for dispatch to the Messenger server. Reconstructed from the original binary.
/// </summary>
public sealed class InternalSessionData
{
    public IRequestInput RequestInput { get; set; }
    public IObject RequestData { get; set; }
    public uint ObjectId { get; set; }

    public InternalSessionData(IRequestInput requestInput, IObject requestData)
    {
        RequestInput = requestInput;
        RequestData = requestData;
        ObjectId = requestInput.ObjectId;
    }
}
