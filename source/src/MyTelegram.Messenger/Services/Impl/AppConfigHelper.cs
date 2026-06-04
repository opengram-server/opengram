namespace MyTelegram.Messenger.Services.Impl;
public partial class AppConfigHelper : IAppConfigHelper, ISingletonDependency
{
    public AppConfigHelper()
    {
        GetAppConfig();
    }

    public int GetAppConfigHash()
    {
        return _hash;
    }

    private void AddCustomConfig()
    {
        SetConfig("stars_paid_messages_available", new TJsonBool { Value = true });
        SetConfig("stars_paid_message_amount_max", new TJsonNumber { Value = 10000 });
        SetConfig("hidden_members_group_size_min", new TJsonNumber { Value = 10 });
    }

    private void SetConfig(string key, IJSONValue value)
    {
        var oldValue = _jsonValue?.Value.FirstOrDefault(p => p.Key == key);
        if (oldValue != null)
        {
            _jsonValue?.Value.Remove(oldValue);
        }
        _jsonValue?.Value.Add(new TJsonObjectValue
        {
            Key = key,
            Value = value
        });
    }
}