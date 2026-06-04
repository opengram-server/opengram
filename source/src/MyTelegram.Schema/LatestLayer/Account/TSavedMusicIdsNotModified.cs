// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// The list of saved music IDs hasn't changed
/// See <a href="https://corefork.telegram.org/constructor/account.savedMusicIdsNotModified"/>
///</summary>
[TlObject(0xf8d49445)]
public class TSavedMusicIdsNotModified : ISavedMusicIds
{
    public uint ConstructorId => 0xf8d49445;

    public void ComputeFlag()
    {
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
    }
}
