// ReSharper disable All

namespace MyTelegram.Converters.Responses;

internal sealed class InputMediaInvoiceResponseConverter : IInputMediaInvoiceResponseConverter, ITransientDependency
{
    public IInputMedia ToLayeredData(TInputMediaInvoice data)
    {
        return data;
    }

    public int Layer => Layers.LayerLatest;
}