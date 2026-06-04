// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// Adds or removes a song from the current user's profile
/// See <a href="https://corefork.telegram.org/method/account.saveMusic"/>
///</summary>
[TlObject(0xb26732a9)]
public class RequestSaveMusic : IRequest<IBool>
{
    public uint ConstructorId => 0xb26732a9;
    ///<summary>
    /// Flags, see <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">TL conditional fields</a>
    ///</summary>
    public int Flags { get; set; }

    ///<summary>
    /// Whether to remove the song from the profile
    ///</summary>
    public bool Unsave { get; set; }

    ///<summary>
    /// The song to add or remove
    ///</summary>
    public MyTelegram.Schema.IInputDocument Id { get; set; }

    ///<summary>
    /// If set, the song will be added after the song passed in this field
    /// See <a href="https://corefork.telegram.org/type/InputDocument"/>
    ///</summary>
    public MyTelegram.Schema.IInputDocument? AfterId { get; set; }

    public void ComputeFlag()
    {
        if (Unsave) Flags = Flags.SetBit(0);
        if (AfterId != null) Flags = Flags.SetBit(1);
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Flags);
        writer.Write(Id);
        if (Flags.IsBitSet(1)) writer.Write(AfterId);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        Flags = buffer.ReadInt32();
        if (Flags.IsBitSet(0)) Unsave = true;
        Id = buffer.Read<MyTelegram.Schema.IInputDocument>();
        if (Flags.IsBitSet(1)) AfterId = buffer.Read<MyTelegram.Schema.IInputDocument>();
    }
}
