namespace MyTelegram.Services.TLObjectConverters;

public class LayeredService<TLayeredConverter> : ILayeredService<TLayeredConverter>
    where TLayeredConverter : ILayeredConverter
{
    private readonly Dictionary<int, TLayeredConverter> _layeredConverters;
    private readonly int[] _layers;
    private readonly int _minLayer;
    private readonly int _maxLayer;
    private readonly TLayeredConverter _maxLayerConverter;
    private readonly TLayeredConverter _minLayerConverter;

    public LayeredService(IEnumerable<TLayeredConverter> converters)
    {
        var allConverters = converters.OrderBy(p => p.Layer).ToList();
        _layeredConverters = allConverters.ToDictionary(k => k.Layer);
        _layers = allConverters.Select(p => p.Layer).ToArray();

        _minLayer = _layers[0];
        _maxLayer = _layers[^1];

        Converter = allConverters[^1];
        _minLayerConverter = allConverters[0];
        _maxLayerConverter = Converter;
    }

    public TLayeredConverter Converter { get; }

    public TLayeredConverter GetConverter(int layer)
    {
        if (layer == 0 || layer == Converter.Layer)
        {
            return Converter;
        }

        if (_layeredConverters.TryGetValue(layer, out var converter))
        {
            return converter;
        }

        if (layer < _minLayer)
        {
            return _minLayerConverter;
        }

        if (layer > _maxLayer)
        {
            return _maxLayerConverter;
        }

        // Since the total number of layers is relatively small(<100), we can directly use foreach to find the nearest layer
        foreach (var theLayer in _layers)
        {
            if (theLayer > layer)
            {
                return _layeredConverters[theLayer];
            }
        }

        return _maxLayerConverter;
    }
}
