// ReSharper disable All

namespace MyTelegram.Schema.Users;

///<summary>
/// Songs pinned to the user's profile
/// See <a href="https://corefork.telegram.org/constructor/users.savedMusic"/>
///</summary>
[TlObject(0xd5f7b08a)]
public class TSavedMusic : ISavedMusic
{
    public uint ConstructorId => 0xd5f7b08a;

    ///<summary>
    /// Total number of songs in the profile
    ///</summary>
    public int Count { get; set; }

    ///<summary>
    /// Song documents
    ///</summary>
    public TVector<MyTelegram.Schema.IDocument> Documents { get; set; }

    public void ComputeFlag()
    {
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Count);
        writer.Write(Documents);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        Count = buffer.ReadInt32();
        Documents = buffer.Read<TVector<MyTelegram.Schema.IDocument>>();
    }
}
