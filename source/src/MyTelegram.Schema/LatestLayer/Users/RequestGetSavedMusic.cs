// ReSharper disable All

namespace MyTelegram.Schema.Users;

///<summary>
/// Get songs pinned to the user's profile
/// See <a href="https://corefork.telegram.org/method/users.getSavedMusic"/>
///</summary>
[TlObject(0x8b6a6a2c)]
public class RequestGetSavedMusic : IRequest<MyTelegram.Schema.Users.ISavedMusic>
{
    public uint ConstructorId => 0x8b6a6a2c;

    ///<summary>
    /// The user whose profile to fetch
    ///</summary>
    public MyTelegram.Schema.IInputUser Id { get; set; }

    ///<summary>
    /// Offset for pagination
    ///</summary>
    public int Offset { get; set; }

    ///<summary>
    /// Maximum number of results to return
    ///</summary>
    public int Limit { get; set; }

    ///<summary>
    /// Hash for pagination, for more info click <a href="https://corefork.telegram.org/api/offsets#hash-generation">here</a>
    ///</summary>
    public long Hash { get; set; }

    public void ComputeFlag()
    {
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Id);
        writer.Write(Offset);
        writer.Write(Limit);
        writer.Write(Hash);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        Id = buffer.Read<MyTelegram.Schema.IInputUser>();
        Offset = buffer.ReadInt32();
        Limit = buffer.ReadInt32();
        Hash = buffer.ReadInt64();
    }
}
