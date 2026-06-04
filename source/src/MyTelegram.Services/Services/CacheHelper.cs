using System.Collections;
using System.Collections.Concurrent;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Services.Services;

public class CacheHelper<TKey, TValue> : ICacheHelper<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> _caches = new();
    public bool TryAdd(TKey key, TValue value)
    {
        return _caches.TryAdd(key, value);
    }

    public bool TryGetValue(TKey key, out TValue? value)
    {
        return _caches.TryGetValue(key, out value);
    }

    public bool TryRemove(TKey key, out TValue? value)
    {
        return _caches.TryRemove(key, out value);
    }
}

// Alias for backward compatibility
public class MyCircularBuffer<T> : CircularBuffer<T>
{
    public MyCircularBuffer(int capacity) : base(capacity)
    {
    }

    public MyCircularBuffer(int capacity, params T[] items) : base(capacity, items)
    {
    }
}