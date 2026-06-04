// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Defines a file uploaded by the client.
/// See <a href="https://corefork.telegram.org/constructor/InputFile" />
///</summary>
[JsonDerivedType(typeof(TInputFile), nameof(TInputFile))]
[JsonDerivedType(typeof(TInputFileBig), nameof(TInputFileBig))]
[JsonDerivedType(typeof(TInputFileStoryDocument), nameof(TInputFileStoryDocument))]
public interface IInputFile : IObject
{

}
