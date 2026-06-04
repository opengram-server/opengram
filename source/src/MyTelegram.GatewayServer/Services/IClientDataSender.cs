namespace MyTelegram.GatewayServer.Services;

public interface IClientDataSender
{
    Task SendAsync(ReadOnlyMemory<byte> data,
        ClientData clientData);

    Task SendAsync(UnencryptedMessageResponse data);
    Task SendAsync(EncryptedMessageResponse data);
    int EncodeData(EncryptedMessageResponse data, ClientData d, Memory<byte> encodedBytes);
    int EncodeData(UnencryptedMessageResponse data, ClientData d, Memory<byte> encodedBytes);
    int GetEncodedDataMaxLength(int messageDataLength);
}