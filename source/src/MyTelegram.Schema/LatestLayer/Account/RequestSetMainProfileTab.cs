// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// Changes the main profile tab of the current user
/// See <a href="https://corefork.telegram.org/method/account.setMainProfileTab"/>
///</summary>
[TlObject(0x788d7fe3)]
public class RequestSetMainProfileTab : IRequest<IBool>
{
    public uint ConstructorId => 0x788d7fe3;
    
    ///<summary>
    /// The profile tab to set as main
    ///</summary>
    public MyTelegram.Schema.IProfileTab Tab { get; set; }

    public void ComputeFlag()
    {
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Tab);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        Tab = buffer.Read<MyTelegram.Schema.IProfileTab>();
    }
}
