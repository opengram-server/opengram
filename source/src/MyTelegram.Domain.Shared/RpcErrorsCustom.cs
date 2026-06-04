namespace MyTelegram;

/// <summary>
/// Custom RPC errors for Third-Party Verification and other features
/// </summary>
public static class RpcErrorsCustom
{
    public static class RpcErrors400
    {
        public const int ErrorCode = 400;
        
        /// <summary>
        /// Bot verifier cannot modify custom description.
        /// <code>
        /// bots.setCustomVerification
        /// </code>
        /// </summary>
        public static readonly RpcError BotVerificationCannotModifyDescription = new(ErrorCode, "BOT_VERIFICATION_CANNOT_MODIFY_DESCRIPTION");
        
        /// <summary>
        /// Bot verifier settings are invalid.
        /// <code>
        /// bots.setCustomVerification
        /// </code>
        /// </summary>
        public static readonly RpcError BotVerifierInvalid = new(ErrorCode, "BOT_VERIFIER_INVALID");

        /// <summary>
        /// The group call is not scheduled.
        /// <code>
        /// phone.startScheduledGroupCall
        /// </code>
        /// </summary>
        public static readonly RpcError GroupcallNotScheduled = new(ErrorCode, "GROUPCALL_NOT_SCHEDULED");
        
        /// <summary>
        /// User account is already frozen.
        /// </summary>
        public static readonly RpcError UserAlreadyFrozen = new(ErrorCode, "USER_ALREADY_FROZEN");
        
        /// <summary>
        /// User account is not frozen.
        /// </summary>
        public static readonly RpcError UserNotFrozen = new(ErrorCode, "USER_NOT_FROZEN");
        
        /// <summary>
        /// Frozen participant is missing or inaccessible.
        /// </summary>
        public static readonly RpcError FrozenParticipantMissing = new(ErrorCode, "FROZEN_PARTICIPANT_MISSING");
    }
    
    public static class RpcErrors420
    {
        public const int ErrorCode = 420;
        
        /// <summary>
        /// This method is not allowed for frozen accounts.
        /// </summary>
        public static readonly RpcError FrozenMethodInvalid = new(ErrorCode, "FROZEN_METHOD_INVALID");
    }
}
