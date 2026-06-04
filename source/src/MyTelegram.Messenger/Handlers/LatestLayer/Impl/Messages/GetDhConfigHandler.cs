// ReSharper disable All

using MyTelegram.Domain.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Returns configuration parameters for Diffie-Hellman key generation. Can also return a random sequence of bytes of required length.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 RANDOM_LENGTH_INVALID Random length invalid.
/// See <a href="https://corefork.telegram.org/method/messages.getDhConfig" />
///</summary>
internal sealed class GetDhConfigHandler(
    IDiffieHellmanService dhService) : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetDhConfig, MyTelegram.Schema.Messages.IDhConfig>,
    Messages.IGetDhConfigHandler
{
    protected override Task<MyTelegram.Schema.Messages.IDhConfig> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetDhConfig obj)
    {
        // Validate random length
        if (obj.RandomLength < 0 || obj.RandomLength > 1024)
        {
            RpcErrors.RpcErrors400.RandomLengthInvalid.ThrowRpcError();
        }
        
        // Get DH config from service
        var config = dhService.GetDhConfig();
        
        // Generate random bytes if requested
        var randomBytes = new byte[obj.RandomLength];
        if (obj.RandomLength > 0)
        {
            Random.Shared.NextBytes(randomBytes);
        }
        
        // Return DH configuration
        return Task.FromResult<MyTelegram.Schema.Messages.IDhConfig>(new MyTelegram.Schema.Messages.TDhConfig
        {
            G = config.Generator,
            P = config.Prime,
            Version = config.Version,
            Random = randomBytes
        });
    }
}
