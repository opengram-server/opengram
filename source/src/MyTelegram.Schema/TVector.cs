namespace MyTelegram.Schema;

[TlObject(0x1cb5c415)]
public class TVector<T> : IObject, IList<T>
{
    private readonly List<T> _list;
    private static readonly ISerializer<T> Serializer = SerializerFactory.CreateSerializer<T>();

    public TVector()
    {
        _list = [];
    }

    public TVector(IEnumerable<T> collection)
    {
        _list = new List<T>(collection);
    }

    public TVector(params T[] items)
    {
        _list = new List<T>(items);
    }

    public uint ConstructorId => 0x1cb5c415;
    public void Serialize(IBufferWriter<byte> writer)
    {
        writer.Write(ConstructorId);
        writer.Write(_list.Count);
        foreach (var item in _list)
        {
            Serializer.Serialize(item, writer);
        }
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var count = buffer.ReadInt32();
        if (count > 0)
        {
            _list.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                var item = Serializer.Deserialize(ref buffer);
                _list.Add(item);
            }
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        _list.Add(item);
    }

    public void AddRange(IEnumerable<T> items)
    {
        _list.AddRange(items);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array,
        int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _list.Remove(item);
    }

    public int Count => _list.Count;
    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index,
        T item)
    {
        _list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    public T this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }
}