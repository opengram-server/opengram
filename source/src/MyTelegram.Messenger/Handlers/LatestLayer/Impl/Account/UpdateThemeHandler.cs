// ReSharper disable All

using MyTelegram.Domain.Aggregates.Theme;
using MyTelegram.Domain.Commands.Theme;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Update theme
/// <para>Possible errors</para>
/// Code Type Description
/// 400 THEME_INVALID Invalid theme provided.
/// See <a href="https://corefork.telegram.org/method/account.updateTheme" />
///</summary>
internal sealed class UpdateThemeHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    ILogger<UpdateThemeHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUpdateTheme, MyTelegram.Schema.ITheme>,
    Account.IUpdateThemeHandler
{
    protected override async Task<MyTelegram.Schema.ITheme> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUpdateTheme obj)
    {
        // Resolve theme identity
        long themeId;
        string? themeSlug = null;

        switch (obj.Theme)
        {
            case MyTelegram.Schema.TInputTheme inputTheme:
                themeId = inputTheme.Id;
                break;
            case MyTelegram.Schema.TInputThemeSlug inputThemeSlug:
                themeSlug = inputThemeSlug.Slug;
                themeId = 0;
                break;
            default:
                throw new RpcException(RpcErrors.RpcErrors400.ThemeInvalid);
        }

        // Validate title length if provided
        if (obj.Title != null && (obj.Title.Length == 0 || obj.Title.Length > 128))
        {
            throw new RpcException(RpcErrors.RpcErrors400.ThemeTitleInvalid);
        }

        // Try to find theme. We look up from the query first.
        // Use existing chat themes query to find the matching theme.
        var chatThemes = await queryProcessor.ProcessAsync(new GetAllChatThemesQuery(), CancellationToken.None);
        Theme? existingTheme = null;

        if (themeSlug != null)
        {
            existingTheme = chatThemes?.FirstOrDefault(t => t.Slug == themeSlug);
        }
        else
        {
            existingTheme = chatThemes?.FirstOrDefault(t => t.Id == themeId);
        }

        if (existingTheme == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ThemeInvalid);
        }

        // Extract document ID if provided
        long? documentId = null;
        if (obj.Document is MyTelegram.Schema.TInputDocument inputDoc)
        {
            documentId = inputDoc.Id;
        }

        var aggregateId = new ThemeId($"theme-{existingTheme.Id}");

        var requestInfo = new RequestInfo(
            ReqMsgId: input.ReqMsgId,
            UserId: input.UserId,
            AccessHashKeyId: 0,
            AuthKeyId: input.AuthKeyId,
            PermAuthKeyId: input.PermAuthKeyId,
            RequestId: Guid.NewGuid(),
            Layer: input.Layer,
            Date: DateTime.UtcNow.Ticks,
            DeviceType: input.DeviceType,
            AddRequestIdToCache: false
        );

        var command = new UpdateThemeCommand(
            aggregateId,
            requestInfo,
            obj.Title,
            obj.Slug,
            documentId,
            emoticon: null);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Theme updated: Id={ThemeId}, Title={Title} by user {UserId}",
            existingTheme.Id, obj.Title ?? existingTheme.Title, input.UserId);

        // Build response with updated values
        TVector<IThemeSettings>? settings = null;
        if (obj.Settings is { Count: > 0 })
        {
            settings = new TVector<IThemeSettings>();
            foreach (var inputSetting in obj.Settings)
            {
                if (inputSetting is MyTelegram.Schema.TInputThemeSettings its)
                {
                    settings.Add(new TThemeSettings
                    {
                        BaseTheme = its.BaseTheme,
                        AccentColor = its.AccentColor,
                        OutboxAccentColor = its.OutboxAccentColor,
                        MessageColors = its.MessageColors,
                        MessageColorsAnimated = its.MessageColorsAnimated
                    });
                }
            }
        }

        return new TTheme
        {
            Id = existingTheme.Id,
            AccessHash = existingTheme.AccessHash,
            Creator = true,
            Default = existingTheme.Default,
            ForChat = existingTheme.ForChat,
            Slug = obj.Slug ?? existingTheme.Slug,
            Title = obj.Title ?? existingTheme.Title,
            Emoticon = existingTheme.Emoticon,
            Settings = settings,
            InstallsCount = 0
        };
    }
}
