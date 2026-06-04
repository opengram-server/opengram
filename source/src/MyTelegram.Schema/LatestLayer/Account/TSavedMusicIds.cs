// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// List of IDs of songs added to the profile
/// See <a href="https://corefork.telegram.org/constructor/account.savedMusicIds"/>
///</summary>
[TlObject(0x9e638c0d)]
public class TSavedMusicIds : ISavedMusicIds
{
    public uint ConstructorId => 0x9e638c0d;

    ///<summary>
    /// List of document IDs
    ///</summary>
    public TVector<long> Ids { get; set; }

    public void ComputeFlag()
    {
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Ids);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        Ids = buffer.Read<TVector<long>>();
    }
}
