// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Buffers;

namespace Redzen.Buffers;

/// <summary>
/// Wraps an array rented from an <see cref="ArrayPool{T}"/>, and exposes a specified number of elements from it as
/// a <see cref="Span{T}"/>.
/// </summary>
/// <typeparam name="T">Array/Span element type.</typeparam>
/// <remarks>
/// This type is <see cref="IDisposable"/>; calling <see cref="IDisposable.Dispose"/> will result in the wrapped
/// rented array being returned to <see cref="ArrayPool{T}.Shared"/>.
/// </remarks>
public readonly struct RentedArray<T> : IDisposable
{
    private readonly int _length;
    private readonly T[] _arr;

    /// <summary>
    /// Constructs a new <see cref="RentedArray{T}"/> that provides a <see cref="Span{T}"/> of the specified length.
    /// </summary>
    /// <param name="length">The length of the required span.</param>
    public RentedArray(int length)
    {
        if(length < 0) throw new ArgumentOutOfRangeException(nameof(length));
        _length = length;
        _arr = ArrayPool<T>.Shared.Rent(length);
    }

    /// <summary>
    /// Creates a new span of the required length over wrapped rented array.
    /// </summary>
    /// <returns>A new <see cref="Span{T}"/>.</returns>
    public Span<T> AsSpan()
    {
        return _arr.AsSpan(0, _length);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_arr);
    }
}