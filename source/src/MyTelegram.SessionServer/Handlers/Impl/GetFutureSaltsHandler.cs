using MyTelegram.Schema;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles get_future_salts — returns upcoming server salts for the session.
/// Reconstructed from the original binary.
/// </summary>
public sealed class GetFutureSaltsHandler : ISessionHandler<RequestGetFutureSalts, IFutureSalts>
{
    private readonly IServerSaltHelper _serverSaltHelper;
    private readonly ILogger<GetFutureSaltsHandler> _logger;

    public GetFutureSaltsHandler(
        IServerSaltHelper serverSaltHelper,
        ILogger<GetFutureSaltsHandler> logger)
    {
        _serverSaltHelper = serverSaltHelper;
        _logger = logger;
    }

    public async Task<IFutureSalts> HandleAsync(IRequestInput input, RequestGetFutureSalts request)
    {
        var count = Math.Clamp(request.Num, 1, 64);
        var salts = await _serverSaltHelper
            .GetOrCreateCachedFutureSaltsAsync(input.AuthKeyId, count)
            .ConfigureAwait(false);

        var now = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var result = new TFutureSalts
        {
            ReqMsgId = input.ReqMsgId,
            Now = now,
            Salts = new TVector<IFutureSalt>(
                salts.Select(s => (IFutureSalt)new TFutureSalt
                {
                    ValidSince = s.ValidSince,
                    ValidUntil = s.ValidUntil,
                    Salt = s.Salt
                }).ToList())
        };

        _logger.LogDebug("GetFutureSalts: authKey={AuthKeyId} returned {Count} salts",
            input.AuthKeyId, result.Salts.Count);

        return result;
    }
}
