using System.Text.Json.Serialization;

namespace MyTelegram;

public class TimeZoneItem(string id, string name, int utcOffset)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;

    [JsonPropertyName("utc_offset")]
    public int UtcOffset { get; set; } = utcOffset;
}