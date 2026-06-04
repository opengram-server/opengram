namespace MyTelegram.Converters.Responses;

public class MessageMediaResponseService(
    IDocumentAttributeVideoResponseService documentAttributeVideoResponseService,
    IObjectMapper objectMapper,
    ILayeredService<IMessageMediaDocumentResponseConverter> messageMediaDocumentLayeredService,
    ILayeredService<IMessageMediaGiveawayResponseConverter> messageMediaGiveawayLayeredService,
    ILayeredService<IMessageMediaGiveawayResultsResponseConverter> messageMeidaGiveawayResultsLayeredService,
    ILayeredService<IInputMediaDocumentExternalResponseConverter> inputMediaDocumentExternalLayeredService,
    ILayeredService<IInputMediaDocumentResponseConverter> inputMediaDocumentLayeredService,
    ILayeredService<IInputMediaInvoiceResponseConverter> inputMediaInvoiceLayeredService,
    ILayeredService<IInputMediaPaidMediaResponseConverter> inputMediaPaidMediaLayeredService,
    ILayeredService<IInputMediaUploadedDocumentResponseConverter> inputMediaUploadedDocumentLayeredService)
    : IMessageMediaResponseService, ITransientDependency
{
    public IMessageMedia? ToLayeredData(IMessageMedia? latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TMessageMediaGiveaway messageMediaGiveaway:
                return messageMediaGiveawayLayeredService.GetConverter(layer).ToLayeredData(messageMediaGiveaway);
            case TMessageMediaGiveawayResults messageMediaGiveawayResults:
                return messageMeidaGiveawayResultsLayeredService.GetConverter(layer)
                    .ToLayeredData(messageMediaGiveawayResults);
            //case Schema.Layer187.TMessageMediaDocument messageMediaDocument187:
            //case Schema.Layer197.TMessageMediaDocument messageMediaDocument198:

            //    break;

            case TMessageMediaDocument messageMediaDocument:
                var newMessageMediaDocument = (TMessageMediaDocument)messageMediaDocumentLayeredService.GetConverter(Layers.LayerLatest).ToLayeredData(messageMediaDocument);
                if (newMessageMediaDocument.Document is TDocument document)
                {
                    var attributes = documentAttributeVideoResponseService.ToLayeredData(document.Attributes, layer);
                    var newDocument = objectMapper.Map<TDocument, TDocument>(document);
                    newDocument.Attributes = attributes;
                    newMessageMediaDocument.Document = newDocument;
                }

                //var altDocuments = newMessageMediaDocument.AltDocuments ?? [];
                if (newMessageMediaDocument.AltDocuments?.Count > 0)
                {
                    var altDocuments = new TVector<IDocument>();
                    foreach (var doc in newMessageMediaDocument.AltDocuments)
                    {
                        if (doc is TDocument altDocument)
                        {
                            var newAltDocument = objectMapper.Map<TDocument, TDocument>(altDocument);
                            var attributes =
                                documentAttributeVideoResponseService.ToLayeredData(newAltDocument.Attributes, layer);
                            newAltDocument.Attributes = attributes;
                            altDocuments.Add(newAltDocument);
                        }
                        else
                        {
                            altDocuments.Add(doc);
                        }
                    }

                    newMessageMediaDocument.AltDocuments = altDocuments;
                }

                return messageMediaDocumentLayeredService.GetConverter(layer).ToLayeredData(newMessageMediaDocument);
        }

        return latestLayerData;
    }

    public IInputMedia? ToLayeredData(IInputMedia? latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TInputMediaUploadedDocument inputMediaUploadedDocument:
                inputMediaUploadedDocument.Attributes =
                    documentAttributeVideoResponseService.ToLayeredData(inputMediaUploadedDocument.Attributes, layer);
                return inputMediaUploadedDocumentLayeredService.GetConverter(layer)
                    .ToLayeredData(inputMediaUploadedDocument);

            case TInputMediaDocument inputMediaDocument:
                return inputMediaDocumentLayeredService.GetConverter(layer).ToLayeredData(inputMediaDocument);

            case TInputMediaDocumentExternal inputMediaDocumentExternal:
                return inputMediaDocumentExternalLayeredService.GetConverter(layer)
                    .ToLayeredData(inputMediaDocumentExternal);

            case TInputMediaPaidMedia inputMediaPaidMedia:
                return inputMediaPaidMediaLayeredService.GetConverter(layer).ToLayeredData(inputMediaPaidMedia);

            case TInputMediaInvoice inputMediaInvoice:
                return inputMediaInvoiceLayeredService.GetConverter(layer).ToLayeredData(inputMediaInvoice);
        }

        return latestLayerData;
    }
}