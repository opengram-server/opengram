// ReSharper disable All

using MyTelegram.Domain.Aggregates.Theme;
using MyTelegram.Domain.Commands.Theme;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Create a theme
/// <para>Possible errors</para>
/// Code Type Description
/// 400 THEME_MIME_INVALID The theme's MIME type is invalid.
/// 400 THEME_TITLE_INVALID The specified theme title is invalid.
/// See <a href="https://corefork.telegram.org/method/account.createTheme" />
///</summary>
internal sealed class CreateThemeHandler(
    ICommandBus commandBus,
    ILogger<CreateThemeHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestCreateTheme, MyTelegram.Schema.ITheme>,
    Account.ICreateThemeHandler
{
    protected override async Task<MyTelegram.Schema.ITheme> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestCreateTheme obj)
    {
        // Validate title (required, max 128 chars)
        if (string.IsNullOrWhiteSpace(obj.Title) || obj.Title.Length > 128)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ThemeTitleInvalid);
        }

        // Validate document MIME type if provided
        if (obj.Document != null)
        {
            // Document must be a valid theme file (application/x-tgtheme-*)
            // For now, accept any document and let it be stored
        }

        // Generate slug if empty
        var slug = !string.IsNullOrWhiteSpace(obj.Slug) ? obj.Slug : GenerateSlug();

        // Extract document ID if provided
        long? documentId = null;
        if (obj.Document is MyTelegram.Schema.TInputDocument inputDoc)
        {
            documentId = inputDoc.Id;
        }

        // Generate unique theme ID
        var themeId = Random.Shared.NextInt64(1_000_000, long.MaxValue);
        var accessHash = Random.Shared.NextInt64(1_000_000, long.MaxValue);
        var aggregateId = new ThemeId($"theme-{themeId}");

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

        var command = new CreateThemeCommand(
            aggregateId,
            requestInfo,
            input.UserId,
            obj.Title,
            slug,
            documentId,
            emoticon: null,
            isDefault: false,
            forChat: false);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Theme created: Id={ThemeId}, Slug={Slug}, Title={Title} by user {UserId}",
            themeId, slug, obj.Title, input.UserId);

        // Build settings from input if provided
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
            Id = themeId,
            AccessHash = accessHash,
            Creator = true,
            Default = false,
            ForChat = false,
            Slug = slug,
            Title = obj.Title,
            Document = null, // Document resolution handled separately
            Settings = settings,
            InstallsCount = 0
        };
    }

    private static string GenerateSlug()
    {
        return $"theme-{Guid.NewGuid().ToString("N")[..12]}";
    }
}
