namespace MyTelegram.Messenger.Converters.ConverterServices;

public class DraftConverterService(ILayeredService<IDraftMessageConverter> draftMessageLayeredService,
    IMessageMediaResponseService messageMediaResponseService
) : ITransientDependency, IDraftConverterService
{
    public IDraftMessage ToDraftMessage(IDraftReadModel draftReadModel,int layer)
    {
        var draftMessage= draftMessageLayeredService.GetConverter(layer).ToDraftMessage(draftReadModel);
        draftMessage.Media = messageMediaResponseService.ToLayeredData(draftReadModel.Draft.Media2, layer);

        return draftMessage;
    }
}