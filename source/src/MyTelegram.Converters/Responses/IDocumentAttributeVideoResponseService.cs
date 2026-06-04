namespace MyTelegram.Converters.Responses;

public interface IDocumentAttributeVideoResponseService
{
    IDocumentAttribute ToLayeredData(IDocumentAttribute latestLayerData, int layer);
    TVector<IDocumentAttribute> ToLayeredData(TVector<IDocumentAttribute> latestLayerData, int layer);
}