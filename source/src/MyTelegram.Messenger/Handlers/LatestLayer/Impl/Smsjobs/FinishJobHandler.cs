namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Smsjobs;

///<summary>
/// Finish an SMS job (official clients only).
/// <para>Possible errors</para>
/// Code Type Description
/// 400 SMSJOB_ID_INVALID The specified job ID is invalid.
/// See <a href="https://corefork.telegram.org/method/smsjobs.finishJob" />
///</summary>
internal sealed class FinishJobHandler : RpcResultObjectHandler<MyTelegram.Schema.Smsjobs.RequestFinishJob, IBool>,
    Smsjobs.IFinishJobHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Smsjobs.RequestFinishJob obj)
    {
        throw new NotImplementedException();
    }
}
