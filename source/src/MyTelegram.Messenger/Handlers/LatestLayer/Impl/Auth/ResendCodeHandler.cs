namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Auth;

///<summary>
/// Resend the login code via another medium.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PHONE_NUMBER_INVALID The phone number is invalid.
/// 400 SEND_CODE_UNAVAILABLE Returned when all available options for this type of number were already used.
/// See <a href="https://corefork.telegram.org/method/auth.resendCode" />
///</summary>
internal sealed class ResendCodeHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Auth.RequestResendCode, MyTelegram.Schema.Auth.ISentCode>,
        Auth.IResendCodeHandler
{
    protected override Task<MyTelegram.Schema.Auth.ISentCode> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Auth.RequestResendCode obj)
    {
        // Code resending requires SMS/call infrastructure
        throw new RpcException(new RpcError(400, "SEND_CODE_UNAVAILABLE"));
    }
}
