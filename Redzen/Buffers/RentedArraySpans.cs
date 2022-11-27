// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Buffers;

namespace Redzen.Buffers;

/// <summary>
/// Wraps an array rented from an <see cref="ArrayPool{T}"/> and exposes one or more segments within the array as
/// spans.
/// </summary>
/// <typeparam name="T">Array/Span element type.</typeparam>
/// <remarks>
/// This type is <see cref="IDisposable"/>; calling <see cref="IDisposable.Dispose"/> will result in the wrapped
/// rented array being returned to the <see cref="ArrayPool{T}"/>.
/// </remarks>
public readonly struct RentedArraySpans<T> : IDisposable
{
    private readonly Memory<T>[] _segments;
    private readonly T[] _arr;

    /// <summary>
    /// Constructs a new <see cref="RentedArraySpans{T}"/> that provides multiple spans of the specified lengths.
    /// </summary>
    /// <param name="lengths">Specifies the lengths of the required spans. The number of lengths determine the number
    /// of require spans.</param>
    public RentedArraySpans(
        Span<int> lengths)
    {
        int sumLengths = 0;
        for(int i=0; i<lengths.Length; i++)
        {
            if(lengths[i] < 0)
                throw new ArgumentException("One or more of the provided length values is negative.", nameof(lengths));

            sumLengths += lengths[i];
        }

        // Rent an array that will be used as the storage for all of the required spans.
        _arr = ArrayPool<T>.Shared.Rent(sumLengths);

        // Create the Memory<T> segments.
        _segments = new Memory<T>[lengths.Length];

        for(int i = 0, startIdx = 0; i < lengths.Length; i++)
        {
            _segments[i] = new Memory<T>(_arr, startIdx, lengths[i]);
            startIdx += lengths[i];
        }
    }

    /// <summary>
    /// Get a span for the specified segment.
    /// </summary>
    /// <param name="segmentIdx">The segment index to get the span for.</param>
    /// <returns>A span over the specified segment's elements.</returns>
    public Span<T> GetSpan(int segmentIdx)
    {
        if (segmentIdx < 0 || segmentIdx >= _segments.Length) throw new ArgumentOutOfRangeException(nameof(segmentIdx));

        return _segments[segmentIdx].Span;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_arr);
    }
}
