// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Represents the gifts tab of a profile page
///</summary>
[TlObject(0x7c5de01b)]
public class TProfileTabGifts : IProfileTab
{
    public uint ConstructorId => 0x7c5de01b;
    
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
