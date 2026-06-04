// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Represents the music tab of a profile page
///</summary>
[TlObject(0x4b2bb6ac)]
public class TProfileTabMusic : IProfileTab
{
    public uint ConstructorId => 0x4b2bb6ac;
    
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
