using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Buffers.Binary;
using System.IO.Compression;

namespace MyTelegram.Services.Services;

public class GZipHelper : IGZipHelper, ITransientDependency
{
    public void Compress(ReadOnlySpan<byte> source, IBufferWriter<byte> writer)
    {
        using var stream = new BufferWriterStream(writer);
        using var gzip = new GZipStream(stream, CompressionLevel.Optimal, true);
        gzip.Write(source);
    }

    public void Decompress(ReadOnlyMemory<byte> source, Span<byte> destination, out int count)
    {
        if (IsGzipPacked(source.Span))
        {
            GzipDecompress(source, destination, out count);
        }
        else
        {
            Unzip(source, destination, out count);
        }
    }

    public static void GzipDecompress(ReadOnlyMemory<byte> source, Span<byte> destination, out int count)
    {
        count = 0;

        using var input = new ReadOnlyMemoryStream(source);
        using var gzip = new GZipStream(input, CompressionMode.Decompress, true);

        while (count < destination.Length)
        {
            var buffer = destination[count..];
            var bytesRead = gzip.Read(buffer);
            if (bytesRead == 0)
            {
                break;
            }

            count += bytesRead;
        }
    }

    public static void Unzip(ReadOnlyMemory<byte> input, Span<byte> destination, out int count)
    {
        count = 0;

        using var inputStream = new ReadOnlyMemoryStream(input);
        using var deflateStream = new InflaterInputStream(inputStream);

        while (count < destination.Length)
        {
            var buffer = destination[count..];
            var bytesRead = deflateStream.Read(buffer);
            if (bytesRead == 0)
            {
                break;
            }

            count += bytesRead;
        }
    }

    public bool TryGetUncompressedLength(ReadOnlySpan<byte> data, out int length)
    {
        if (IsGzipPacked(data))
        {
            return TryGetGzipUncompressedLength(data, out length);
        }

        return TryGetZipUncompressedLength(data, out length);
    }


    public bool TryGetGzipUncompressedLength(ReadOnlySpan<byte> data, out int length)
    {
        if (data.Length < 4)
        {
            length = 0;
            return false;
        }

        uint len32 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(data.Length - 4));

        if (len32 > int.MaxValue)
        {
            length = 0;
            return false;
        }

        length = (int)len32;
        return true;
    }

    public bool TryGetZipUncompressedLength(ReadOnlySpan<byte> data, out int length)
    {
        if (data.Length < 22)
        {
            length = 0;
            return false;
        }

        // Check local file header signature (first 4 bytes)
        if (!data.Slice(0, 4).SequenceEqual(new byte[] { 0x50, 0x4B, 0x03, 0x04 }))
        {
            length = 0;
            return false;
        }

        uint len32 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(18, 4));
        if (len32 > int.MaxValue)
        {
            length = 0;
            return false;
        }

        length = (int)len32;
        return true;
    }

    private static bool IsGzipPacked(ReadOnlySpan<byte> data)
    {
        return data.Length >= 3 &&
               data[0] == 0x1f &&
               data[1] == 0x8b &&
               data[2] == 0x08;
    }

    public sealed class BufferWriterStream(IBufferWriter<byte> writer) : Stream
    {
        private readonly IBufferWriter<byte> _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        private Memory<byte> _currentMemory = writer.GetMemory();
        private int _position;

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if ((uint)offset > (uint)buffer.Length || (uint)count > (uint)(buffer.Length - offset))
            {
                throw new ArgumentOutOfRangeException();
            }

            Write(buffer.AsSpan(offset, count));
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            while (!buffer.IsEmpty)
            {
                if (_position >= _currentMemory.Length)
                {
                    FlushInternal();
                }

                var writable = Math.Min(_currentMemory.Length - _position, buffer.Length);
                buffer.Slice(0, writable).CopyTo(_currentMemory.Span.Slice(_position));
                _position += writable;
                buffer = buffer.Slice(writable);
            }
        }

        public override void WriteByte(byte value)
        {
            if (_position >= _currentMemory.Length)
            {
                FlushInternal();
            }

            _currentMemory.Span[_position++] = value;
        }

        private void FlushInternal()
        {
            if (_position > 0)
            {
                _writer.Advance(_position);
                _currentMemory = _writer.GetMemory();
                _position = 0;
            }
        }

        public override void Flush()
        {
            FlushInternal();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                FlushInternal();
            }

            base.Dispose(disposing);
        }

        #region Stream abstract members

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    private sealed class ReadOnlyMemoryStream(ReadOnlyMemory<byte> span) : Stream
    {
        private int _position;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => span.Length;

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan(offset, count));
        }

        public override int Read(Span<byte> buffer)
        {
            var remaining = span.Length - _position;
            if (remaining <= 0)
            {
                return 0;
            }

            var toCopy = Math.Min(buffer.Length, remaining);
            span.Span.Slice(_position, toCopy).CopyTo(buffer);
            _position += toCopy;
            return toCopy;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}