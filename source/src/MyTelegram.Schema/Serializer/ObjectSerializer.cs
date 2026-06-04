namespace MyTelegram.Schema.Serializer;

public class ObjectSerializer<T> : ISerializer<T>
{
    private const uint VectorConstructorId = 0x1cb5c415;

    public void Serialize(T value,
        IBufferWriter<byte> writer)
    {
        if (value is IObject tlObject)
        {
            tlObject.Serialize(writer);
        }
        else
        {
            throw new NotSupportedException($"Only support `IObject`, but input type is `{typeof(T).Name}` ");
        }
    }

    public T Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var constructorId = buffer.ReadUInt32();
        
        var typeOfT = typeof(T);
        if (!SerializerObjectMappings.TryGetTlObject(constructorId, out var func))
        {
            SerializerObjectMappings.TryGetTlObject(typeOfT, out func);
        }

        if (func != null)
        {
            var obj = func();
            obj.Deserialize(ref buffer);

            return (T)obj;
        }

        if (typeOfT.IsInterface)
        {
            // typeOfT=IObject,targetType=TVector<TObjectOrSubTypes>
            if (constructorId == VectorConstructorId)
            {
                // [0..4]=constructorId [4..8]=_list.Count [8..12]=constructorId of T (Vector<T>)
                // Read 4 bytes from 8 to 12,the first 4 bytes already read by reader.ReadUInt32()
                // So need to read 4 bytes from 4 to 8
                var vectorOfTConstructorIdBytesSpan = buffer[4..8].Span;
                var constructorId2 = BitConverter.ToUInt32(vectorOfTConstructorIdBytesSpan);
                if (SerializerObjectMappings.TryGetTlObjectType(constructorId2, out var type))
                {
                    var baseType = typeof(IObject);
                    var interfaceType = type.GetInterfaces()
                        .FirstOrDefault(p => p.IsAssignableTo(baseType) && p.FullName != baseType.FullName);

                    var genericType = interfaceType ?? type;

                    var genericVectorType = typeof(TVector<>).MakeGenericType(genericType);

                    if (!SerializerObjectMappings.TryGetTlObject(genericVectorType, out var vectorOfObjFunc))
                    {
                        vectorOfObjFunc = MyReflectionHelper.CompileConstructor<IObject>(genericVectorType);
                        SerializerObjectMappings.TryAddTlObjectFuncToCache(genericVectorType, vectorOfObjFunc);
                    }

                    var vectorOfObj = vectorOfObjFunc();
                    vectorOfObj.Deserialize(ref buffer);

                    return (T)vectorOfObj;
                }
            }

            // All supported constructor ids are loaded in memory when first time access SerializerObjectMappings
            // if can not find object type by constructorId, may be telegram client version is mismatch
            
            // Для неизвестных конструкторов InputPeer возвращаем InputPeerEmpty, чтобы не ломать отправку сообщений
            if (typeOfT == typeof(IInputPeer))
            {
                System.Diagnostics.Debug.WriteLine($"[SCHEMA WARNING] Unknown InputPeer constructorId:0x{constructorId:x}, returning InputPeerEmpty");
                var emptyPeer = new TInputPeerEmpty();
                return (T)(object)emptyPeer;
            }
            
            throw new NotSupportedException($"Unsupported constructorId:0x{constructorId:x2},type:{typeOfT.Namespace}.{typeOfT.Name}");
        }

        // typeOfT=typeof(TVector<T>)
        if (!SerializerObjectMappings.TryGetTlObject(typeOfT, out var vectorObjFunc))
        {
            vectorObjFunc = MyReflectionHelper.CompileConstructor<IObject>(typeOfT);
            SerializerObjectMappings.TryAddTlObjectFuncToCache(typeOfT, vectorObjFunc);
        }

        var vectorObj = vectorObjFunc();
        vectorObj.Deserialize(ref buffer);

        return (T)vectorObj;
    }
}