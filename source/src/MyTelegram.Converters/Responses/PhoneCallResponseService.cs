namespace MyTelegram.Converters.Responses;

public class PhoneCallResponseService(
    ILayeredService<IPhoneCallResponseConverter> phoneCallLayeredService,
    ILayeredService<IPhoneCallAcceptedResponseConverter> phoneCallAcceptedLayeredService,
    ILayeredService<IPhoneCallDiscardedResponseConverter> phoneCallDiscardedLayeredService,
    ILayeredService<IPhoneCallRequestedResponseConverter> phoncallRequestedLayeredService,
    ILayeredService<IPhoneCallWaitingResponseConverter> phoneCallWaitingLayeredService)
    : IPhoneCallResponseService, ITransientDependency
{
    public IPhoneCall ToLayeredData(IPhoneCall latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TPhoneCall phoneCall:
                return phoneCallLayeredService.GetConverter(layer).ToLayeredData(phoneCall);

            case TPhoneCallAccepted phoneCallAccepted:
                return phoneCallAcceptedLayeredService.GetConverter(layer).ToLayeredData(phoneCallAccepted);

            case TPhoneCallDiscarded phoneCallDiscarded:
                return phoneCallDiscardedLayeredService.GetConverter(layer).ToLayeredData(phoneCallDiscarded);

            case TPhoneCallRequested phoneCallRequested:
                return phoncallRequestedLayeredService.GetConverter(layer).ToLayeredData(phoneCallRequested);

            case TPhoneCallWaiting phoneCallWaiting:
                return phoneCallWaitingLayeredService.GetConverter(layer).ToLayeredData(phoneCallWaiting);
        }

        return latestLayerData;
    }
}