namespace MyTelegram.Converters.Responses;

public class DocumentAttributeVideoResponseService(
    ILayeredService<IDocumentAttributeVideoResponseConverter> documentAttributeVideoLayeredService)
    : IDocumentAttributeVideoResponseService, ITransientDependency
{
    public IDocumentAttribute ToLayeredData(IDocumentAttribute latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TDocumentAttributeVideo documentAttributeVideo:
                return documentAttributeVideoLayeredService.GetConverter(layer).ToLayeredData(documentAttributeVideo);
        }

        return latestLayerData;
    }

    public TVector<IDocumentAttribute> ToLayeredData(TVector<IDocumentAttribute> latestLayerData, int layer)
    {
        var newAttributes = new List<IDocumentAttribute>(latestLayerData.Count);
        foreach (var documentAttribute in latestLayerData)
        {
            newAttributes.Add(ToLayeredData(documentAttribute, layer));
        }

        return new TVector<IDocumentAttribute>(newAttributes);
    }
}