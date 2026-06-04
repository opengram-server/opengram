namespace MyTelegram.Core;

public record LayeredData<TData>(Dictionary<int, TData>? DataWithLayer);