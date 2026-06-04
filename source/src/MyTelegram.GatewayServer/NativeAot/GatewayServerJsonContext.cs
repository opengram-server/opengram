using System.Text.Json.Serialization;
namespace MyTelegram.GatewayServer.NativeAot;

[JsonSerializable(typeof(EncryptedMessage))]
[JsonSerializable(typeof(EncryptedMessageResponse))]
[JsonSerializable(typeof(UnencryptedMessage))]
[JsonSerializable(typeof(UnencryptedMessageResponse))]
[JsonSerializable(typeof(AuthKeyNotFoundEvent))]
[JsonSerializable(typeof(ClientDisconnectedEvent))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class GatewayServerJsonContext : JsonSerializerContext
{
}