namespace MyTelegram.Services.Services;

public interface ICacheHelper<in TKey, TValue>
{
    bool TryAdd(TKey key, TValue value);
    bool TryGetValue(TKey key, out TValue? value);
    bool TryRemove(TKey key, out TValue? value);
}