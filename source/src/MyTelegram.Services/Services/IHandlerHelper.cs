using System.Diagnostics.CodeAnalysis;

namespace MyTelegram.Services.Services;

public interface IHandlerHelper
{
    void InitAllHandlers();

    bool TryGetHandler(uint objectId,
        [NotNullWhen(true)] out IObjectHandler? handler);

    bool TryGetHandlerName(uint objectId,
        [NotNullWhen(true)] out string? handlerName);

    bool TryGetHandlerShortName(uint objectId,
        [NotNullWhen(true)] out string? handlerShortName);

    string GetHandlerFullName(IObject requestData);
}