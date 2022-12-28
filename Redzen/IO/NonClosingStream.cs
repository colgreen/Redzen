// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.IO;

/// <summary>
/// Wraps a stream and prevents calls to Close() and Dispose() from being made on it.
/// This is useful for other classes that wrap a stream but have no ability to leave
/// the wrapped stream open upon Dispose(), e.g. CryptoStream and BinaryWriter.
///
/// Note. Later versions of the .NET framework have added a 'leaveOpen' option
/// to some classes thus eliminating the need for a class such as this in those instances.
/// </summary>
public sealed class NonClosingStream : Stream
{
#pragma warning disable CA2213 // Disposable fields should be disposed
    readonly Stream _innerStream;
#pragma warning restore CA2213
    bool _isClosed;

    #region Constructor

    /// <summary>
    /// Construct with the provided stream to be wrapped.
    /// </summary>
    /// <param name="stream">The stream to be wrapped.</param>
    public NonClosingStream(Stream stream)
    {
        _innerStream = stream;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the wrapped stream.
    /// </summary>
    public Stream InnerStream => _innerStream;

    #endregion

    #region Properties [Overrides]

    /// <inheritdoc/>
    public override bool CanRead => !_isClosed && _innerStream.CanRead;

    /// <inheritdoc/>
    public override bool CanSeek => !_isClosed && _innerStream.CanSeek;

    /// <inheritdoc/>
    public override bool CanWrite => !_isClosed && _innerStream.CanWrite;

    /// <inheritdoc/>
    public override long Length
    {
        get
        {
            CheckClosed();
            return _innerStream.Length;
        }
    }

    /// <inheritdoc/>
    public override long Position
    {
        get
        {
            CheckClosed();
            return _innerStream.Position;
        }

        set
        {
            CheckClosed();
            _innerStream.Position = value;
        }
    }

    #endregion

    #region Public Methods [Overrides]

    /// <inheritdoc/>
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        CheckClosed();
        return _innerStream.BeginRead(buffer, offset, count, callback, state);
    }

    /// <inheritdoc/>
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        CheckClosed();
        return _innerStream.BeginWrite(buffer, offset, count, callback, state);
    }

    /// <inheritdoc/>
    public override void Close()
    {
        if(!_isClosed)
            _innerStream.Flush();

        _isClosed = true;
    }

    /// <inheritdoc/>
    public override int EndRead(IAsyncResult asyncResult)
    {
        CheckClosed();
        return _innerStream.EndRead(asyncResult);
    }

    /// <inheritdoc/>
    public override void EndWrite(IAsyncResult asyncResult)
    {
        CheckClosed();
        _innerStream.EndWrite(asyncResult);
    }

    /// <inheritdoc/>
    public override void Flush()
    {
        CheckClosed();
        _innerStream.Flush();
    }

    /// <inheritdoc/>
    public override int Read(Span<byte> buffer)
    {
        CheckClosed();
        return _innerStream.Read(buffer);
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        CheckClosed();
        return _innerStream.Read(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override int ReadByte()
    {
        CheckClosed();
        return _innerStream.ReadByte();
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        CheckClosed();
        return _innerStream.Seek(offset, origin);
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        CheckClosed();
        _innerStream.SetLength(value);
    }

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        CheckClosed();
        _innerStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        CheckClosed();
        _innerStream.Write(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override void WriteByte(byte value)
    {
        CheckClosed();
        _innerStream.WriteByte(value);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Throws an InvalidOperationException if the wrapper is closed.
    /// </summary>
    private void CheckClosed()
    {
        if(_isClosed)
            throw new InvalidOperationException("The stream has been closed.");
    }

    #endregion
}
