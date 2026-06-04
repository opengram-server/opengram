// ReSharper disable All

using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Commands.UserPassword;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Obtain configuration for two-factor authorization with password
/// See <a href="https://corefork.telegram.org/method/account.getPassword" />
///</summary>
internal sealed class GetPasswordHandler : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetPassword, MyTelegram.Schema.Account.IPassword>,
    Account.IGetPasswordHandler
{
    private readonly IRandomHelper _randomHelper;
    private readonly IQueryProcessor _queryProcessor;
    private readonly ICommandBus _commandBus;
    private readonly ISrpService _srpService;
    private readonly ILogger<GetPasswordHandler> _logger;

    public GetPasswordHandler(
        IRandomHelper randomHelper,
        IQueryProcessor queryProcessor,
        ICommandBus commandBus,
        ISrpService srpService,
        ILogger<GetPasswordHandler> logger)
    {
        _randomHelper = randomHelper;
        _queryProcessor = queryProcessor;
        _commandBus = commandBus;
        _srpService = srpService;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.Account.IPassword> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetPassword obj)
    {
        var password = new TPassword();
        var secureRandom = new byte[256];
        _randomHelper.NextBytes(secureRandom);
        password.SecureRandom = secureRandom;

        // Query password read model
        var passwordReadModel = await _queryProcessor
            .ProcessAsync(new GetUserPasswordQuery(input.UserId), default);

        if (passwordReadModel == null || !passwordReadModel.HasPassword)
        {
            // No password set - return empty configuration
            password.HasPassword = false;
            password.HasRecovery = false;
            password.HasSecureValues = false;

            // Provide default algorithm for setting new password
            var (g, p) = _srpService.GetDefaultSrpParams();
            password.NewAlgo = new TPasswordKdfAlgoSHA256SHA256PBKDF2HMACSHA512iter100000SHA256ModPow
            {
                Salt1 = _srpService.GenerateSalt(32),
                Salt2 = _srpService.GenerateSalt(32),
                G = g,
                P = p
            };
            password.NewSecureAlgo = new TSecurePasswordKdfAlgoUnknown();

            return password;
        }

        // User has password - generate SRP session
        _logger.LogInformation("Generating SRP session for user {UserId}, V length: {VLen}, G: {G}, P length: {PLen}",
            input.UserId, passwordReadModel.V?.Length, passwordReadModel.G, passwordReadModel.P?.Length);
        
        var (srpId, srpB, srpBPrivate) = _srpService.GenerateServerSrpSession(
            passwordReadModel.V!,
            passwordReadModel.G,
            passwordReadModel.P!);

        _logger.LogInformation("Generated SRP session - SrpId: {SrpId}, B length: {BLen}, bPrivate length: {BPrivLen}",
            srpId, srpB.Length, srpBPrivate.Length);

        // Store SRP session
        var command = new CreateSrpSessionCommand(
            UserPasswordId.Create(input.UserId),
            input.ToRequestInfo(),
            input.UserId,
            srpId,
            srpB,
            srpBPrivate);

        await _commandBus.PublishAsync(command);

        // Build response
        password.HasPassword = true;
        password.HasRecovery = !string.IsNullOrEmpty(passwordReadModel.RecoveryEmail);
        password.HasSecureValues = passwordReadModel.HasSecureValues;
        password.Hint = passwordReadModel.Hint;
        password.EmailUnconfirmedPattern = passwordReadModel.EmailUnconfirmedPattern;
        password.LoginEmailPattern = passwordReadModel.LoginEmailPattern;
        password.PendingResetDate = passwordReadModel.PendingResetDate;

        // Current password algorithm
        password.CurrentAlgo = new TPasswordKdfAlgoSHA256SHA256PBKDF2HMACSHA512iter100000SHA256ModPow
        {
            Salt1 = passwordReadModel.Salt1!,
            Salt2 = passwordReadModel.Salt2!,
            G = passwordReadModel.G,
            P = passwordReadModel.P!
        };

        // SRP session data
        password.SrpId = srpId;
        password.SrpB = srpB;

        // Algorithm for setting new password
        var (newG, newP) = _srpService.GetDefaultSrpParams();
        password.NewAlgo = new TPasswordKdfAlgoSHA256SHA256PBKDF2HMACSHA512iter100000SHA256ModPow
        {
            Salt1 = _srpService.GenerateSalt(32),
            Salt2 = _srpService.GenerateSalt(32),
            G = newG,
            P = newP
        };
        password.NewSecureAlgo = new TSecurePasswordKdfAlgoUnknown();

        return password;
    }
}
