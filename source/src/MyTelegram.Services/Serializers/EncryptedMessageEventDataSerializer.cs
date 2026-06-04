//namespace MyTelegram.Services.Serializers;

//public class EncryptedMessageEventDataSerializer : IEventDataSerializer<EncryptedMessage>, ITransientDependency
//{
//    public object? Deserialize(Type type, ReadOnlyMemory<byte> buffer)
//    {
//        return Deserialize(buffer);
//    }

//    public void Serialize(IBufferWriter<byte> writer, EncryptedMessage data)
//    {
//        writer.Write(data.AuthKeyId);
//        writer.Write(data.MsgKey);
//        writer.Write(data.EncryptedData);
//        writer.WriteString(data.ConnectionId);
//        writer.Write((byte)data.ConnectionType);
//        writer.WriteString(data.ClientIp);
//        writer.Write(data.RequestId);
//        writer.Write(data.Date);
//    }

//    public EncryptedMessage Deserialize(ReadOnlyMemory<byte> buffer)
//    {
//        var authKeyId = buffer.ReadInt64();
//        var msgKey = buffer.ReadMemory();
//        var encryptedData = buffer.ReadMemory();
//        var connectionId = buffer.ReadString2() ?? string.Empty;
//        var connectionType = (ConnectionType)buffer.ReadByte();
//        var clientIp = buffer.ReadString2() ?? string.Empty;
//        var requestId = buffer.ReadGuid();
//        var date = buffer.ReadInt64();
//        var owner = MemoryPool<byte>.Shared.Rent(msgKey.Length + encryptedData.Length);
//        var memory = owner.Memory;
//        var msgKeyMemory = memory.Slice(0, msgKey.Length);
//        var encryptedDataMemory = memory.Slice(msgKey.Length, encryptedData.Length);
//        msgKey.CopyTo(msgKeyMemory);
        
//        encryptedData.CopyTo(encryptedDataMemory);

//        return new EncryptedMessage(authKeyId, msgKeyMemory, encryptedDataMemory, connectionId, connectionType,
//            clientIp, requestId, date)
//        {
//            // Released by the caller
//            MemoryOwner = owner
//        };
//    }
//}