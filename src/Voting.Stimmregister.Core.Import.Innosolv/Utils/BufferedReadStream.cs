// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Core.Import.Innosolv.Utils;

/// <summary>
/// This stream buffers the first N bytes from a non-seekable stream in memory.
/// This allows to read the first N bytes multiple times.
/// </summary>
internal class BufferedReadStream : Stream
{
    private readonly Stream _underlyingStream;
    private readonly int _maxBufferSize;
    private readonly MemoryStream _buffer = new();
    private int _position;
    private bool _bufferNeeded = true;

    internal BufferedReadStream(Stream underlyingStream, int maxBufferSize)
    {
        _underlyingStream = underlyingStream;
        _maxBufferSize = maxBufferSize;
    }

    public override bool CanRead => _underlyingStream.CanRead;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => _underlyingStream.Length;

    public override long Position
    {
        get => _position;
        set => throw new NotSupportedException("Setting the position is not supported for this stream.");
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        // Check if we are currently reading from the buffer
        if (_position < _buffer.Length)
        {
            var bufferBytesRead = _buffer.Read(buffer, offset, count);
            _position += bufferBytesRead;
            return bufferBytesRead;
        }

        // If we get here, check if we need to buffer data from the underlying stream
        if (_bufferNeeded)
        {
            var bytesRead = _underlyingStream.Read(buffer, offset, count);
            _position += bytesRead;

            // This is mostly a safeguard against buffering too much of the underlying stream
            if (_position >= _maxBufferSize)
            {
                throw new InvalidOperationException($"Max buffer size of {_maxBufferSize} exceeded.");
            }

            _buffer.Write(buffer, offset, bytesRead);
            return bytesRead;
        }

        // If we get here, we are reading directly from the underlying stream without any buffering
        return _underlyingStream.Read(buffer, offset, count);
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => await ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        // Check if we are currently reading from the buffer
        if (_position < _buffer.Length)
        {
            var bufferBytesRead = await _buffer.ReadAsync(buffer, cancellationToken);
            _position += bufferBytesRead;
            return bufferBytesRead;
        }

        // If we get here, check if we need to buffer data from the underlying stream
        if (_bufferNeeded)
        {
            var bytesRead = await _underlyingStream.ReadAsync(buffer, cancellationToken);
            _position += bytesRead;

            // This is mostly a safeguard against buffering too much of the underlying stream
            if (_position >= _maxBufferSize)
            {
                throw new InvalidOperationException($"Max buffer size of {_maxBufferSize} exceeded.");
            }

            await _buffer.WriteAsync(buffer[..bytesRead], cancellationToken);
            return bytesRead;
        }

        // If we get here, we are reading directly from the underlying stream without any buffering
        return await _underlyingStream.ReadAsync(buffer, cancellationToken);
    }

    public void SeekToStartOfBuffer()
    {
        _position = 0;
        _buffer.Seek(0, SeekOrigin.Begin);

        // Disable buffering, we do not support to do it multiple times
        _bufferNeeded = false;
    }

    public void StopBuffering()
    {
        _bufferNeeded = false;
    }

    public override void Flush()
        => _underlyingStream.Flush();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException("Stream does not support seeking.");

    public override void SetLength(long value)
        => throw new NotSupportedException("Stream does not support setting the length.");

    public override void Write(byte[] buffer, int offset, int count)
        => throw new NotSupportedException("Stream does not support writing.");

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _underlyingStream.DisposeAsync();
        await _buffer.DisposeAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _underlyingStream.Dispose();
            _buffer.Dispose();
        }

        base.Dispose(disposing);
    }
}
