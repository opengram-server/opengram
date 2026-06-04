using System.Text.Json;

namespace MyTelegram.EventFlow;

public interface ISystemTextJsonOptions
{
    void Apply(JsonSerializerOptions settings);
}