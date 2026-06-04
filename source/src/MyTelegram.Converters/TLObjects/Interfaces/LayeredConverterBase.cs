namespace MyTelegram.Converters.TLObjects.Interfaces;

public abstract class LayeredConverterBase : ILayeredConverter
{
    
    public abstract int Layer { get; }

    //protected int GetLayer()
    //{
    //    return RequestLayer > 0 ? RequestLayer : Layer;
    //}
}