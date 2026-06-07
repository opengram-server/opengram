using System.IO.Compression;
using MyTelegram.Schema;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles gzip_packed — decompresses the payload and re-dispatches.
/// Reconstructed from the original binary.
/// </summary>
public sealed class GzipPackedHandler : IUnwrappingSessionHandler<TGzipPacked>
{
    private readonly ILogger<GzipPackedHandler> _logger;

    public GzipPackedHandler(ILogger<GzipPackedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(IRequestInput input, TGzipPacked request,
        Func<IRequestInput, IObject, Task> dispatch)
    {
        if (request.PackedData.IsEmpty)
        {
            _logger.LogWarning("Empty gzip_packed data from authKey={AuthKeyId}", input.AuthKeyId);
            return;
        }

        try
        {
            using var compressedStream = new MemoryStream(request.PackedData.ToArray());
            using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            await gzipStream.CopyToAsync(decompressedStream).ConfigureAwait(false);

            var decompressedData = new ReadOnlyMemory<byte>(decompressedStream.ToArray());
            var serializer = SerializerFactory.CreateSerializer<IObject>();
            var innerObject = serializer.Deserialize(ref decompressedData);

            if (innerObject != null)
            {
                _logger.LogDebug("GzipPacked decompressed: {TypeName} authKey={AuthKeyId}",
                    innerObject.GetType().Name, input.AuthKeyId);
                await dispatch(input, innerObject).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decompress gzip_packed from authKey={AuthKeyId}", input.AuthKeyId);
        }
    }
}
