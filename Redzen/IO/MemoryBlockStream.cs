// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.IO;

/// <summary>
/// A memory backed stream that stores byte data in blocks, this gives improved performance over System.IO.MemoryStream
/// in some circumstances.
///
/// MemoryStream is backed by a single byte array, hence if the capacity is reached a new byte array must be instantiated
/// and the existing data copied across. In contrast, MemoryBlockStream grows in blocks and therefore avoids copying and
/// re-instantiating large byte arrays.
///
/// By using a sufficiently small block size, the blocks will avoid being placed onto the large object heap (LOH), with
/// various benefits, e.g. avoidance/mitigation of memory fragmentation.
///
/// Also consider using <see href="https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream"/>, which addresses
/// the same issues/concerns as this class, but is far more advanced.
/// </summary>
public sealed class MemoryBlockStream : Stream
{
    readonly int _blockSize;

    // Indicates if the stream is open.
    bool _isOpen;

    // The read/write position within the stream; note that this can be moved back to write over existing data.
    int _position;

    // Stream length. Indicates where the end of the stream is.
    int _length;

    readonly List<byte[]> _blockList;

    /// <summary>
    /// Construct with the default block size.
    /// </summary>
    public MemoryBlockStream()
        : this(8192)
    {
    }

    /// <summary>
    /// Construct with the specified block size.
    /// </summary>
    /// <param name="blockSize">The block size to use.</param>
    public MemoryBlockStream(int blockSize)
    {
        _isOpen = true;
        _position = 0;
        _length = 0;
        _blockSize = blockSize;
        _blockList = new List<byte[]>(100);
    }

    /// <summary>
    /// Gets the MemoryBlockStream current capacity.
    /// </summary>
    private int Capacity => _blockList.Count * _blockSize;

    /// <inheritdoc/>
    public override bool CanRead => _isOpen;

    /// <inheritdoc/>
    public override bool CanSeek => _isOpen;

    /// <inheritdoc/>
    public override bool CanWrite => _isOpen;

    /// <inheritdoc/>
    public override long Position
    {
        get
        {
            ObjectDisposedException.ThrowIf(!_isOpen, this);
            return _position;
        }

        set
        {
            if(value < 0L)
                throw new ArgumentOutOfRangeException(nameof(value), "Number must be either non-negative and less than or equal to Int32.MaxValue or -1.");

            ObjectDisposedException.ThrowIf(!_isOpen, this);

            if(value > (long)int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value), "Stream length must be non-negative and less than 2^31-1.");

            if(value > _length)
                EnsureCapacity((int)value);

            _position = (int)value;
        }
    }

    /// <inheritdoc/>
    public override long Length
    {
        get
        {
            ObjectDisposedException.ThrowIf(!_isOpen, this);
            return _length;
        }
    }

    /// <inheritdoc/>
    public override int Read(Span<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(!_isOpen, this);

        // Test for end of stream (or beyond end).
        if(_position >= _length)
            return 0;

        // Read bytes into the buffer.
        int blockIdx = _position / _blockSize;
        int blockOffset = _position % _blockSize;
        return ReadInner(buffer, blockIdx, blockOffset);
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if(offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Non-negative number required.");
        if(count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
        if((buffer.Length - offset) < count)
            throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

        ObjectDisposedException.ThrowIf(!_isOpen, this);

        // Test for end of stream (or beyond end).
        if(_position >= _length)
            return 0;

        // Read bytes into the buffer.
        int blockIdx = _position / _blockSize;
        int blockOffset = _position % _blockSize;

        var span = buffer.AsSpan(offset, count);
        return ReadInner(span, blockIdx, blockOffset);
    }

    /// <inheritdoc/>
    public override int ReadByte()
    {
        ObjectDisposedException.ThrowIf(!_isOpen, this);

        // Test for end of stream (or beyond end).
        if(_position >= _length)
            return -1;

        // Read byte.
        int blkIdx = _position / _blockSize;
        int blkOffset = _position++ % _blockSize;
        return _blockList[blkIdx][blkOffset];
    }

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(!_isOpen, this);

        if(buffer.Length == 0)
        {
            // Note. In principle there should be nothing to do here, i.e. no state change to the memory stream.
            // However, MemoryStream *will* update its state here in one specific scenario, where the Position is
            // beyond the end of the stream (i.e. greater than Length), Writing zero bytes will cause the Length
            // to increase to match the Position.
            if(_position > _length)
                _length = _position;

            return;
        }

        // Determine new position (post write).
        int endPos = _position + buffer.Length;

        // Check for overflow
        if(endPos < 0) throw new IOException("Stream was too long.");

        // Ensure there are enough blocks ready to write all of the provided data into.
        EnsureCapacity(endPos);

        // Write the bytes into the stream.
        int blockIdx = _position / _blockSize;
        int blockOffset = _position % _blockSize;
        WriteInner(buffer, blockIdx, blockOffset);
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if(offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Non-negative number required.");
        if(count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
        if((buffer.Length - offset) < count)
            throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

        ObjectDisposedException.ThrowIf(!_isOpen, this);

        if(count == 0)
        {
            // Note. In principle there should be nothing to do here, i.e. no state change to the memory stream.
            // However, MemoryStream *will* update its state here in one specific scenario, where the Position is
            // beyond the end of the stream (i.e. greater than Length), Writing zero bytes will cause the Length
            // to increase to match the Position.
            if(_position > _length)
                _length = _position;

            return;
        }

        // Determine new position (post write).
        int endPos = _position + count;

        // Check for overflow
        if(endPos < 0) throw new IOException("Stream was too long.");

        // Ensure there are enough blocks ready to write all of the provided data into.
        EnsureCapacity(endPos);

        // Write the bytes into the stream.
        int blockIdx = _position / _blockSize;
        int blockOffset = _position % _blockSize;
        WriteInner(buffer.AsSpan(offset, count), blockIdx, blockOffset);
    }

    /// <inheritdoc/>
    public override void WriteByte(byte value)
    {
        ObjectDisposedException.ThrowIf(!_isOpen, this);

        // Determine new position (post write).
        int endPos = _position + 1;

        // Check for overflow
        if(endPos < 0) throw new IOException("Stream was too long.");

        // Ensure there is capacity to write the byte into.
        EnsureCapacity(endPos);

        // Write the byte into the stream.
        int blkIdx = _position / _blockSize;
        int blkOffset = _position % _blockSize;
        _blockList[blkIdx][blkOffset] = value;

        // Update state.
        _position++;
        if(_position > _length)
            _length = _position;
    }

    /// <inheritdoc/>
    public override void Flush()
    {   // Memory based stream with nothing to flush; Do nothing.
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        ObjectDisposedException.ThrowIf(!_isOpen, this);
        if(offset > (long)int.MaxValue) throw new ArgumentOutOfRangeException(nameof(offset), "Stream length must be non-negative and less than 2^31-1.");

        switch(origin)
        {
            case SeekOrigin.Begin:
                {
                    if(offset < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");
                    _position = (int)offset;
                    break;
                }
            case SeekOrigin.Current:
                {
                    int newPos = unchecked(_position + (int)offset);
                    if(newPos < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");
                    _position = newPos;
                    break;
                }
            case SeekOrigin.End:
                {
                    int newPos = unchecked(_length + (int)offset);
                    if(newPos < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");
                    _position = newPos;
                    break;
                }
            default:
                {
                    throw new ArgumentException("Invalid seek origin.");
                }
        }
        return _position;
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        if(value < 0 || value > Int32.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value), "Stream length must be non-negative and less than 2^31 - 1.");

        int newLength = (int)value;
        if(newLength == _length)
        {   // Do nothing.
            return;
        }

        if(newLength > _length)
        {
            // Handle case where new length is beyond the current length.
            // Ensure that any existing capacity after _length is zeroed.
            ZeroSpareCapacity();

            // Grow the capacity to ensure _length is within the bounds of allocated space that can be read.
            // Note. newly created blocks are zeroed by default.
            EnsureCapacity(newLength);

            // Set new length.
            _length = newLength;
        }
        else if(newLength < _length)
        {
            _length = newLength;
        }

        // 'Snap back' the position. This is done to mimic the behaviour of MemoryStream, although the reason for doing this is
        // unclear since setting Position directly allows a position beyond the end of the stream.
        if(_position > newLength)
            _position = newLength;
    }

    /// <summary>
    /// Writes the stream contents to a byte array, regardless of the Position property.
    /// </summary>
    /// <returns>A new byte array.</returns>
    public byte[] ToArray()
    {
        // Allocate new byte array.
        byte[] buff = new byte[_length];
        int buffIdx = 0;

        // Calc number of full blocks.
        int fullBlockCount = _length / _blockSize;

        // Loop full blocks, copying them into buff as we go.
        for(int i=0; i < fullBlockCount; i++)
        {
            byte[] blk = _blockList[i];
            Array.Copy(blk, 0, buff, buffIdx, _blockSize);
            buffIdx += _blockSize;
        }

        // Handle final block possibly/probably partially filled.
        int tailCount = _length % _blockSize;
        if(tailCount != 0)
        {
            byte[] blk = _blockList[fullBlockCount];
            Array.Copy(blk, 0, buff, buffIdx, tailCount);
        }

        return buff;
    }

    /// <summary>
    /// Remove excess blocks from the block list.
    /// </summary>
    public void Trim()
    {
        int currBlockCount = _blockList.Count;
        int newBlockCount = _length / _blockSize;
        if((_length % _blockSize) != 0)
            newBlockCount++;

        if(newBlockCount < currBlockCount)
            _blockList.RemoveRange(newBlockCount, currBlockCount-newBlockCount);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _isOpen = false;
        base.Dispose(disposing);
    }

    private void EnsureCapacity(int value)
    {
        if(value > Capacity)
        {
            int blockCount = (value / _blockSize) + 1;
            int createCount = blockCount - _blockList.Count;
            for(int i=0; i < createCount; i++)
                _blockList.Add(new byte[_blockSize]);
        }
    }

    /// <summary>
    /// Read bytes from the memory stream into the provided buffer, starting at the specified block index and intra-block offset..
    /// </summary>
    private int ReadInner(Span<byte> buff, int blockIdx, int blockOffset)
    {
        // Determine how many bytes will be read (based on requested bytes versus the number available).
        int readCount = Math.Min(buff.Length, _length - _position);
        if(readCount == 0)
            return 0;

        int remaining = readCount;
        int tgtOffset = 0;
        int blkIdx = blockIdx;
        int blkOffset = blockOffset;

        while(true)
        {
            // Get handle on memory stream block.
            byte[] blk = _blockList[blkIdx];

            // Determine how many bytes to read from the block.
            int copyCount = Math.Min(_blockSize - blkOffset, remaining);

            // Write bytes into the buffer.
            blk.AsSpan(blkOffset, copyCount).CopyTo(buff[tgtOffset..]);

            // Test for completion.
            remaining -= copyCount;
            if(remaining == 0)
            {   // All bytes have been copied.
                break;
            }

            // Update state.
            tgtOffset += copyCount;
            blkIdx++;
            blkOffset = 0;
        }

        _position += readCount;
        return readCount;
    }

    /// <summary>
    /// Write bytes into the memory stream, starting at the specified block index and intra-block offset.
    /// </summary>
    private void WriteInner(ReadOnlySpan<byte> buff, int blockIdx, int blockOffset)
    {
        int remaining = buff.Length;
        int srcOffset = 0;
        int blkIdx = blockIdx;
        int blkOffset = blockOffset;

        while(true)
        {
            // Get handle on target block.
            byte[] blk = _blockList[blkIdx];

            // Determine how many bytes to write into this block.
            int copyCount = Math.Min(_blockSize - blkOffset, remaining);

            // Write bytes into the block.
            buff.Slice(srcOffset, copyCount).CopyTo(blk.AsSpan(blkOffset));

            // Test for completion.
            remaining -= copyCount;
            if(remaining == 0)
            {   // All bytes have been copied.
                break;
            }

            // Update state.
            srcOffset += copyCount;
            blkIdx++;
            blkOffset = 0;
        }

        _position += buff.Length;
        if(_position > _length)
            _length = _position;
    }

    /// <summary>
    /// Ensure that any existing capacity after _length is zeroed.
    /// </summary>
    private void ZeroSpareCapacity()
    {
        // Handle tail of the first block to zero/reset.
        int blockIdx = _length / _blockSize;
        if(blockIdx == _blockList.Count)
            return;

        int blockOffset = _length % _blockSize;
        byte[] blk = _blockList[blockIdx];
        Array.Clear(blk, blockOffset, _blockSize - blockOffset);

        // Zero any further blocks.
        for(blockIdx++; blockIdx < _blockList.Count; blockIdx++)
        {
            blk = _blockList[blockIdx];
            Array.Clear(blk, 0, _blockSize);
        }
    }
}
