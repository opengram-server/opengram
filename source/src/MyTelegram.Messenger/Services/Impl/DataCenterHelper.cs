namespace MyTelegram.Messenger.Services.Impl;

public class DataCenterHelper(IOptions<MyTelegramMessengerServerOptions> options)
    : IDataCenterHelper, ITransientDependency
{
    public int GetMediaDcId()
    {
        //var dcId=_options.Value.IsMediaDc
        var defaultDcId = MyTelegramConsts.MediaDcId;
        var dc = options.Value.DcOptions?.FirstOrDefault(p => p.Id == defaultDcId);
        if (dc != null)
        {
            return defaultDcId;
        }

        return options.Value.ThisDcId;
    }
}