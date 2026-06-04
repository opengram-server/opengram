// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Get phone call configuration to be passed to libtgvoip's shared config
/// See <a href="https://corefork.telegram.org/method/phone.getCallConfig" />
///</summary>
internal sealed class GetCallConfigHandler : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestGetCallConfig, MyTelegram.Schema.IDataJSON>,
    Phone.IGetCallConfigHandler
{
    protected override Task<MyTelegram.Schema.IDataJSON> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestGetCallConfig obj)
    {
        // Return VoIP configuration as JSON
        // This is passed to libtgvoip/tgcalls library
        var config = new
        {
            tcp_timeout = 15.0,
            udp_timeout = 5.0,
            max_connection_time = 120.0,
            enable_video_send = true,
            enable_video_receive = true,
            enable_p2p = true,
            enable_aec = true,
            enable_ns = true,
            enable_agc = true,
            enable_dtx = true,
            audio_max_bitrate = 20000,
            audio_init_bitrate = 16000,
            video_max_bitrate = 1000000,
            video_init_bitrate = 400000
        };
        
        var json = System.Text.Json.JsonSerializer.Serialize(config);
        
        return Task.FromResult<MyTelegram.Schema.IDataJSON>(new TDataJSON
        {
            Data = json
        });
    }
}
