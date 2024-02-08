// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Voting.Stimmregister.Core.Services.Supporting.Signing.Exceptions;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

internal class HmacSha256Stream : Stream
{
    private readonly AesCipherMetadata _aesCipherMetadata;
    private readonly HMACSHA256 _hmacsha256;
    private readonly Stream _hmacCryptoStream;
    private volatile bool _disposed;

    internal HmacSha256Stream(Stream targetStream, byte[] macKey, AesCipherMetadata aesCipherMetadata)
    {
        if (macKey.Length != 16)
        {
            throw new ArgumentException("MAC key must be of length 16.", nameof(macKey));
        }

        _aesCipherMetadata = aesCipherMetadata;
        _hmacsha256 = new HMACSHA256();
        _hmacsha256.Key = macKey;
        _hmacCryptoStream = new CryptoStream(targetStream, _hmacsha256, CryptoStreamMode.Write);
    }

    public override bool CanRead => _hmacCryptoStream.CanRead;

    public override bool CanSeek => _hmacCryptoStream.CanSeek;

    public override bool CanWrite => _hmacCryptoStream.CanWrite;

    public override long Length => _hmacCryptoStream.Length;

    public override long Position
    {
        get => _hmacCryptoStream.Position;
        set => _hmacCryptoStream.Position = value;
    }

    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await _hmacCryptoStream.DisposeAsync();
        _aesCipherMetadata.Hmac = _hmacsha256.Hash ?? throw new EncryptionException("HMACSHA265 was not computed.");
        _hmacsha256.Dispose();
    }

    public override void Flush()
        => _hmacCryptoStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => _hmacCryptoStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin)
        => _hmacCryptoStream.Seek(offset, origin);

    public override void SetLength(long value)
        => _hmacCryptoStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
        => _hmacCryptoStream.Write(buffer, offset, count);
}
