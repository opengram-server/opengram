namespace MyTelegram;

public class EncryptedFileItem(
    long fileId,
    long accessHash,
    int dcId,
    int size)
{
    public long AccessHash { get; init; } = accessHash;
    public int DcId { get; init; } = dcId;

    public long FileId { get; init; } = fileId;
    public int Size { get; init; } = size;
}