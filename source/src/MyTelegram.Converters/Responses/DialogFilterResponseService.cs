namespace MyTelegram.Converters.Responses;

public class DialogFilterResponseService(
    ILayeredService<IDialogFilterResponseConverter> dialogFilterLayeredService,
    ILayeredService<IDialogFilterChatlistResponseConverter> dialogFilterChatlistLayeredService)
    : IDialogFilterResponseService, ITransientDependency
{
    public IDialogFilter? ToLayeredData(IDialogFilter? latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TDialogFilterChatlist dialogFilterChatlist:
                return dialogFilterChatlistLayeredService.GetConverter(layer).ToLayeredData(dialogFilterChatlist);
            case TDialogFilter dialogFilter:
                return dialogFilterLayeredService.GetConverter(layer).ToLayeredData(dialogFilter);
        }

        return latestLayerData;
    }
}