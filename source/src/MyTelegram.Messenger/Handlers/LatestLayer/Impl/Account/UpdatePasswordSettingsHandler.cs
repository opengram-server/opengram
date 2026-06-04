// ReSharper disable All

using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Commands.UserPassword;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Set a new 2FA password
///</summary>
internal sealed class UpdatePasswordSettingsHandler : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUpdatePasswordSettings, IBool>,
    Account.IUpdatePasswordSettingsHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ICommandBus _commandBus;
    private readonly ISrpService _srpService;
    private readonly IRpcErrorHelper _rpcErrorHelper;
    private readonly ILogger<UpdatePasswordSettingsHandler> _logger;

    public UpdatePasswordSettingsHandler(
        IQueryProcessor queryProcessor,
        ICommandBus commandBus,
        ISrpService srpService,
        IRpcErrorHelper rpcErrorHelper,
        ILogger<UpdatePasswordSettingsHandler> logger)
    {
        _queryProcessor = queryProcessor;
        _commandBus = commandBus;
        _srpService = srpService;
        _rpcErrorHelper = rpcErrorHelper;
        _logger = logger;
    }

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUpdatePasswordSettings obj)
    {
        var passwordReadModel = await _queryProcessor
            .ProcessAsync(new GetUserPasswordQuery(input.UserId), default);

        // Verify old password if user already has one
        if (passwordReadModel?.HasPassword == true)
        {
            if (obj.Password is not TInputCheckPasswordSRP srpPassword)
            {
                new RpcError(400, "PASSWORD_HASH_INVALID").ThrowRpcError();
                return null!;
            }

            if (passwordReadModel.SrpId != srpPassword.SrpId)
            {
                new RpcError(400, "SRP_ID_INVALID").ThrowRpcError();
                return null!;
            }

            var isValid = _srpService.VerifySrpProof(
                srpPassword.A.ToArray(),
                srpPassword.M1.ToArray(),
                passwordReadModel.V!,
                passwordReadModel.SrpBPrivate!,
                passwordReadModel.SrpB!,
                passwordReadModel.G,
                passwordReadModel.P!,
                passwordReadModel.Salt1!,
                passwordReadModel.Salt2!);

            if (!isValid)
            {
                new RpcError(400, "PASSWORD_HASH_INVALID").ThrowRpcError();
                return null!;
            }
        }

        // Check if removing password
        if (obj.NewSettings.NewPasswordHash == null || obj.NewSettings.NewPasswordHash.Length == 0)
        {
            // Remove password
            var removeCommand = new RemovePasswordCommand(
                UserPasswordId.Create(input.UserId),
                input.ToRequestInfo(),
                input.UserId);

            await _commandBus.PublishAsync(removeCommand);
            return new TBoolTrue();
        }

        // Setting/updating password
        if (obj.NewSettings.NewAlgo is not TPasswordKdfAlgoSHA256SHA256PBKDF2HMACSHA512iter100000SHA256ModPow algo)
        {
            new RpcError(400, "NEW_SETTINGS_INVALID").ThrowRpcError();
            return null!;
        }

        _logger.LogInformation("Setting password - Salt1 len: {Salt1Len}, Salt2 len: {Salt2Len}, V len: {VLen}, G: {G}, P len: {PLen}",
            algo.Salt1.Length, algo.Salt2.Length, obj.NewSettings.NewPasswordHash.Length, algo.G, algo.P.Length);

        // The client sends the password hash, but we need to compute v from it
        // According to Telegram's protocol, newPasswordHash is actually the password verifier (v)
        var v = obj.NewSettings.NewPasswordHash;

        // Telegram sends Salt1 - it may be 32 or 64 bytes
        // According to testing: when client sends 64 bytes, we should store ALL 64 bytes
        // because the client uses the full 64 bytes when computing M1
        byte[] salt1 = algo.Salt1.ToArray();
        
        _logger.LogInformation("Received Salt1 length: {Salt1Len} bytes", salt1.Length);
        if (salt1.Length == 64)
        {
            _logger.LogInformation("Salt1 is 64 bytes - storing as-is for M1 verification");
            _logger.LogInformation("Salt1 (first 32): {Salt1First32}", Convert.ToHexString(salt1.Take(32).ToArray()));
            _logger.LogInformation("Salt1 (last 32): {Salt1Last32}", Convert.ToHexString(salt1.Skip(32).Take(32).ToArray()));
        }
        else if (salt1.Length == 32)
        {
            _logger.LogInformation("Salt1 is 32 bytes: {Salt1Hex}", Convert.ToHexString(salt1));
        }
        else
        {
            _logger.LogWarning("Unexpected Salt1 length: {Len} bytes", salt1.Length);
        }
        
        var salt2 = algo.Salt2.ToArray();

        _logger.LogInformation("After processing - Salt1 len: {Salt1Len}, Salt2 len: {Salt2Len}", 
            salt1.Length, salt2.Length);
        _logger.LogInformation("Using Salt1: {Salt1Hex}, Salt2: {Salt2Hex}", 
            Convert.ToHexString(salt1), Convert.ToHexString(salt2));

        var setCommand = new SetPasswordCommand(
            UserPasswordId.Create(input.UserId),
            input.ToRequestInfo(),
            input.UserId,
            salt1,
            salt2,
            v,
            obj.NewSettings.Hint,
            obj.NewSettings.Email,
            algo.G,
            algo.P.ToArray());

        await _commandBus.PublishAsync(setCommand);

        return new TBoolTrue();
    }
}
