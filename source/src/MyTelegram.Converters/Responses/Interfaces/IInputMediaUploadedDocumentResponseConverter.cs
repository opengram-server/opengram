namespace MyTelegram.Converters.Responses.Interfaces;

public interface IInputMediaUploadedDocumentResponseConverter
    : IResponseConverter<
        TInputMediaUploadedDocument,
        IInputMedia
    >
{
    //IInputMedia ToLatestLayerData(MyTelegram.Schema.IInputMedia oldLayerData);
}