//namespace MyTelegram.Services.Serializers;

//public class EventBusRawDataReceivedEventRawDataSerializer : IEventDataSerializer<EventBusRawDataReceivedEvent>, ITransientDependency
//{
//    public object? Deserialize(Type type, ReadOnlyMemory<byte> buffer)
//    {
//        return Deserialize(buffer);
//    }

//    public void Serialize(IBufferWriter<byte> writer, EventBusRawDataReceivedEvent data)
//    {
//        writer.WriteBool(data.Handled);
//        writer.WriteRawBytes(data.RawData);
//    }

//    public EventBusRawDataReceivedEvent Deserialize(ReadOnlyMemory<byte> buffer)
//    {
//        var handled = buffer.ReadBoolean();
//        var data = buffer;

//        return new EventBusRawDataReceivedEvent(null, data, handled);
//    }
//}