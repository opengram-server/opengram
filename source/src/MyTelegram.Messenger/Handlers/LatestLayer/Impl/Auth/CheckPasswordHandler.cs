// ReSharper disable All

using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Commands.UserPassword;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Auth;

///<summary>
/// Try logging to an account protected by a <a href="https://corefork.telegram.org/api/srp">2FA password</a>.
///</summary>
internal sealed class CheckPasswordHandler : RpcResultObjectHandler<MyTelegram.Schema.Auth.RequestCheckPassword, MyTelegram.Schema.Auth.IAuthorization>,
    Auth.ICheckPasswordHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ICommandBus _commandBus;
    private readonly ISrpService _srpService;
    private readonly IRpcErrorHelper _rpcErrorHelper;
    private readonly ILogger<CheckPasswordHandler> _logger;
    private readonly IUserConverterService _userConverterService;
    private readonly ILayeredService<IAuthorizationConverter> _authorizationLayeredService;
    private readonly IPhotoAppService _photoAppService;

    public CheckPasswordHandler(
        IQueryProcessor queryProcessor,
        ICommandBus commandBus,
        ISrpService srpService,
        IRpcErrorHelper rpcErrorHelper,
        ILogger<CheckPasswordHandler> logger,
        IUserConverterService userConverterService,
        ILayeredService<IAuthorizationConverter> authorizationLayeredService,
        IPhotoAppService photoAppService)
    {
        _queryProcessor = queryProcessor;
        _commandBus = commandBus;
        _srpService = srpService;
        _rpcErrorHelper = rpcErrorHelper;
        _logger = logger;
        _userConverterService = userConverterService;
        _authorizationLayeredService = authorizationLayeredService;
        _photoAppService = photoAppService;
    }

    protected override async Task<MyTelegram.Schema.Auth.IAuthorization> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Auth.RequestCheckPassword obj)
    {
        if (obj.Password is not TInputCheckPasswordSRP srpPassword)
        {
            new RpcError(400, "PASSWORD_HASH_INVALID").ThrowRpcError();
            return null!;
        }

        // Get user's password from temp session or by userId
        // Note: During login, userId might not be in input yet, need to get from auth session
        var userId = input.UserId;
        if (userId == 0)
        {
            // Try to get userId from auth key or session
            // For now, throw error - this should be handled in SignIn flow
            new RpcError(400, "AUTH_KEY_UNREGISTERED").ThrowRpcError();
            return null!;
        }

        var passwordReadModel = await _queryProcessor
            .ProcessAsync(new GetUserPasswordQuery(userId), default);

        if (passwordReadModel == null || !passwordReadModel.HasPassword)
        {
            new RpcError(400, "PASSWORD_HASH_INVALID").ThrowRpcError();
            return null!;
        }

        if (passwordReadModel.SrpId != srpPassword.SrpId)
        {
            _logger.LogWarning("Invalid SRP ID. Expected: {Expected}, Got: {Got}",
                passwordReadModel.SrpId, srpPassword.SrpId);
            new RpcError(400, "SRP_ID_INVALID").ThrowRpcError();
            return null!;
        }

        // Verify SRP proof
        _logger.LogInformation("Verifying SRP for user {UserId}, SrpId: {SrpId}, A length: {ALen}, M1 length: {M1Len}, V length: {VLen}, B length: {BLen}",
            userId, srpPassword.SrpId, srpPassword.A.Length, srpPassword.M1.Length, 
            passwordReadModel.V?.Length, passwordReadModel.SrpB?.Length);
        
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
            _logger.LogWarning("Password verification failed for user {UserId}", userId);
            new RpcError(400, "PASSWORD_HASH_INVALID").ThrowRpcError();
            return null!;
        }
        
        _logger.LogInformation("Password verified successfully for user {UserId}", userId);

        // Password verified successfully
        var verifyCommand = new VerifyPasswordCommand(
            UserPasswordId.Create(userId),
            input.ToRequestInfo(),
            userId,
            srpPassword.SrpId,
            srpPassword.A.ToArray(),
            srpPassword.M1.ToArray());

        await _commandBus.PublishAsync(verifyCommand);

        // Complete sign-in process - get user and create authorization
        var userReadModel = await _queryProcessor
            .ProcessAsync(new GetUserByIdQuery(userId), default);

        if (userReadModel == null)
        {
            new RpcError(400, "USER_NOT_FOUND").ThrowRpcError();
            return null!;
        }

        // Get user photos and convert to TL object
        var photos = await _photoAppService.GetPhotosAsync(userReadModel);
        var user = _userConverterService.ToUser(input, userReadModel, photos, layer: input.Layer);

        // Create authorization response
        return _authorizationLayeredService.GetConverter(input.Layer).CreateAuthorization(user);
    }
}
