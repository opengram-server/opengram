namespace MyTelegram.SmsSender;

public class SmsSenderFactory(ILogger<SmsSenderFactory> logger, IEnumerable<ISmsSender> smsSenderList, INullSmsSender nullSmsSender)
    : ISmsSenderFactory, ITransientDependency
{
    private readonly List<ISmsSender> _enabledSmsSenderList = smsSenderList.Where(p => p.Enabled).ToList();

    public ISmsSender Create(string phoneNumber)
    {
        if (_enabledSmsSenderList.Count == 0)
        {
            logger.LogWarning("All SMS sender disabled, SMS will not be sent");
            return nullSmsSender;
        }

        var index = Random.Shared.Next(0, _enabledSmsSenderList.Count);

        return _enabledSmsSenderList[index];
    }
}