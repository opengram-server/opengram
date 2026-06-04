namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Help;

/// <summary>
///     Get app-specific configuration, see
///     <a href="https://corefork.telegram.org/api/config#client-configuration">client configuration</a> for more info on
///     the result.
///     See <a href="https://corefork.telegram.org/method/help.getAppConfig" />
/// </summary>
internal sealed class GetAppConfigHandler(
    IAppConfigHelper appConfigHelper,
    IQueryProcessor queryProcessor) :
    RpcResultObjectHandler<Schema.Help.RequestGetAppConfig, Schema.Help.IAppConfig>,
    Help.IGetAppConfigHandler
{
    protected override async Task<Schema.Help.IAppConfig> HandleCoreAsync(IRequestInput input,
        Schema.Help.RequestGetAppConfig obj)
    {
        var hash = appConfigHelper.GetAppConfigHash();
        if (obj.Hash == hash)
        {
            return new TAppConfigNotModified();
        }
        
        // ВАЖНО: Получаем базовый config
        var baseConfig = appConfigHelper.GetAppConfig();
        
        // Проверяем заморозку пользователя
        var userReadModel = await queryProcessor.ProcessAsync(new GetUserByIdQuery(input.UserId));
        
        // Если пользователь заморожен, создаем КОПИЮ config и добавляем freeze данные
        var config = baseConfig;
        if (userReadModel?.IsFrozen == true)
        {
            var baseJsonValue = baseConfig as TJsonObject;
            if (baseJsonValue?.Value != null)
            {
                // Создаем КОПИЮ TVector
                var newValueList = new TVector<IJSONObjectValue>();
                foreach (var item in baseJsonValue.Value)
                {
                    newValueList.Add(item);
                }
                
                // Add freeze_since_date
                if (userReadModel.FreezeSinceDate.HasValue)
                {
                    newValueList.Add(new TJsonObjectValue
                    {
                        Key = "freeze_since_date",
                        Value = new TJsonNumber { Value = userReadModel.FreezeSinceDate.Value }
                    });
                }

                // Add freeze_until_date
                if (userReadModel.FreezeUntilDate.HasValue)
                {
                    newValueList.Add(new TJsonObjectValue
                    {
                        Key = "freeze_until_date",
                        Value = new TJsonNumber { Value = userReadModel.FreezeUntilDate.Value }
                    });
                }

                // Add freeze_appeal_url
                if (!string.IsNullOrEmpty(userReadModel.FreezeAppealUrl))
                {
                    newValueList.Add(new TJsonObjectValue
                    {
                        Key = "freeze_appeal_url",
                        Value = new TJsonString { Value = userReadModel.FreezeAppealUrl }
                    });
                }
                
                // Создаем новый объект config с freeze данными
                config = new TJsonObject { Value = newValueList };
            }
        }

        var appConfig = new TAppConfig
        {
            Config = config,
            Hash = hash
        };

        return appConfig;
    }
}