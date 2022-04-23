// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen;

/// <summary>
/// Span static utility methods.
/// </summary>
public static class SpanUtils
{
    /// <summary>
    /// Compares the elements of two spans.
    /// </summary>
    /// <param name="x">First span.</param>
    /// <param name="y">Second span.</param>
    /// <typeparam name="T">Span element type.</typeparam>
    /// <returns>True if the contents of the two provided spans are equal; otherwise false.</returns>
    public static bool Equal<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
    {
        // x and y are equal if they point to the same segment of memory, and have the same length.
        if(x == y)
            return true;

        // x and y and not equal if they have different lengths, regardless of whether they point to
        // the same segment of memory or not.
        if(x.Length != y.Length)
            return false;

        // x and y are *content* equals if their contained values are equal, regardless of whether they
        // point to the same segment of memory or not.
        var comp = EqualityComparer<T>.Default;

        for(int i = 0; i < x.Length; i++)
        {
            if(!comp.Equals(x[i], y[i]))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Compares the elements of a span with the single given value.
    /// </summary>
    /// <param name="span">The span to test.</param>
    /// <param name="v">The test value.</param>
    /// <typeparam name="T">Span element type.</typeparam>
    /// <returns>True if the elements of the given span are equal to the given value.</returns>
    public static bool Equal<T>(ReadOnlySpan<T> span, T v)
    {
        var comp = EqualityComparer<T>.Default;
        for(int i=0; i < span.Length; i++)
        {
            if(!comp.Equals(span[i], v))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Randomly shuffles the items of a span.
    /// </summary>
    /// <param name="span">The span to shuffle.</param>
    /// <param name="rng">Random number generator.</param>
    /// <typeparam name="T">The span element type.</typeparam>
    public static void Shuffle<T>(Span<T> span, IRandomSource rng)
    {
        // Fisher–Yates shuffle.
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

        for(int i = span.Length-1; i > 0; i--)
        {
            int swapIdx = rng.Next(i + 1);
            T tmp = span[swapIdx];
            span[swapIdx] = span[i];
            span[i] = tmp;
        }
    }
}
