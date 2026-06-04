using System.Text.Json;
using MyTelegram.Core;

namespace MyTelegram.Caching.Redis;

public class CacheSerializer(JsonSerializerOptions options) : ICacheSerializer
{
    private readonly JsonSerializerOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    public byte[] Serialize<T>(T obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
    }

    public T? Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes, _options);
    }
}