// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Represents the posts tab of a profile page
///</summary>
[TlObject(0x6ca9e978)]
public class TProfileTabPosts : IProfileTab
{
    public uint ConstructorId => 0x6ca9e978;
    
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
