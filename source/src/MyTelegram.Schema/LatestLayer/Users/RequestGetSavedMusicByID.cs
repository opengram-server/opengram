// ReSharper disable All

namespace MyTelegram.Schema.Users;

///<summary>
/// Check if the passed songs are still pinned to the user's profile, or refresh the file references
/// See <a href="https://corefork.telegram.org/method/users.getSavedMusicByID"/>
///</summary>
[TlObject(0x5c7b7e8d)]
public class RequestGetSavedMusicByID : IRequest<MyTelegram.Schema.Users.ISavedMusic>
{
    public uint ConstructorId => 0x5c7b7e8d;

    ///<summary>
    /// The user whose profile to check
    ///</summary>
    public MyTelegram.Schema.IInputUser Id { get; set; }

    ///<summary>
    /// The songs to check
    ///</summary>
    public TVector<MyTelegram.Schema.IInputDocument> Documents { get; set; }

    public void ComputeFlag()
    {
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Id);
        writer.Write(Documents);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        Id = buffer.Read<MyTelegram.Schema.IInputUser>();
        Documents = buffer.Read<TVector<MyTelegram.Schema.IInputDocument>>();
    }
}
