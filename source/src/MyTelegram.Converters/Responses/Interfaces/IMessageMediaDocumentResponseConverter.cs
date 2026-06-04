namespace MyTelegram.Converters.Responses.Interfaces;

public interface IMessageMediaDocumentResponseConverter
    : IResponseConverter<
        TMessageMediaDocument,
        IMessageMedia
    >
{
}