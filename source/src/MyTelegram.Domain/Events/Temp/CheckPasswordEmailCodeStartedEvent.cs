namespace MyTelegram.Domain.Events.Temp;

public class CheckPasswordEmailCodeStartedEvent(RequestInfo requestInfo, string code) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public string Code { get; } = code;
}