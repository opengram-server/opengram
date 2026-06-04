// ReSharper disable All

namespace MyTelegram.Converters.Responses;

public partial interface IInputMediaInvoiceResponseConverter
    : IResponseConverter<
        TInputMediaInvoice,
        IInputMedia
    >
{
}