// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// Fetch the full list of only the IDs of songs currently added to the profile
/// See <a href="https://corefork.telegram.org/method/account.getSavedMusicIds"/>
///</summary>
[TlObject(0xe09d5faf)]
public class RequestGetSavedMusicIds : IRequest<MyTelegram.Schema.Account.ISavedMusicIds>
{
    public uint ConstructorId => 0xe09d5faf;

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
        writer.Write(Hash);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        Hash = buffer.ReadInt64();
    }
}
